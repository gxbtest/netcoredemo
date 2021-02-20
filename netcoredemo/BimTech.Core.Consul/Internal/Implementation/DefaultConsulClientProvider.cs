using BimTech.Core.Consul.Configurations;
using BimTech.Core.Consul.Internal.Cluster.HealthChecks;
using BimTech.Core.Consul.Internal.Cluster.Implementation.Selectors;
using BimTech.Core.CPlatform;
using BimTech.Core.CPlatform.Address;
using BimTech.Core.CPlatform.Runtime.Client.Address.Resolvers.Implementation.Selectors;
using Consul;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BimTech.Core.Consul.Internal.Implementation
{
    public class DefaultConsulClientProvider : IConsulClientProvider
    {
        private ConfigInfo _config;
        private readonly IHealthCheckService _healthCheckService;
        private readonly IConsulAddressSelector _consulAddressSelector;
        private readonly ConcurrentDictionary<string, IAddressSelector> _addressSelectors = new
            ConcurrentDictionary<string, IAddressSelector>();
        private readonly ConcurrentDictionary<AddressModel, ConsulClient> _consulClients = new
            ConcurrentDictionary<AddressModel, ConsulClient>();

        public DefaultConsulClientProvider(ConfigInfo config, IHealthCheckService healthCheckService, IConsulAddressSelector consulAddressSelector)
        {
            _config = config;
            _healthCheckService = healthCheckService;
            _consulAddressSelector = consulAddressSelector;
        }

        public async ValueTask<ConsulClient> GetClient()
        {
            ConsulClient result = null;
            var address = new List<AddressModel>();
            foreach (var addressModel in _config.Addresses)
            {
                _healthCheckService.Monitor(addressModel);
                var task = _healthCheckService.IsHealth(addressModel);
                if (!(task.IsCompletedSuccessfully ? task.Result : await task))
                {
                    continue;
                }
                address.Add(addressModel);
            }

            var vt = _consulAddressSelector.SelectAsync(new AddressSelectContext
            {
                Descriptor = new ServiceDescriptor { Id = nameof(DefaultConsulClientProvider) },
                Address = address
            });
            var addr = vt.IsCompletedSuccessfully ? vt.Result : await vt;
            if (addr != null)
            {
                var ipAddress = addr as IpAddressModel;
                result = _consulClients.GetOrAdd(ipAddress, new ConsulClient(config =>
                {
                    config.Address = new Uri($"http://{ipAddress.Ip}:{ipAddress.Port}");
                }, null, h => { h.UseProxy = false; h.Proxy = null; }));
            }
            return result;
        }

        public async ValueTask<IEnumerable<ConsulClient>> GetClients()
        {
            var result = new List<ConsulClient>();
            foreach (var address in _config.Addresses)
            {
                var ipAddress = address as IpAddressModel;
                if (await _healthCheckService.IsHealth(address))
                {
                    result.Add(_consulClients.GetOrAdd(ipAddress, new ConsulClient(config =>
                    {
                        config.Address = new Uri($"http://{ipAddress.Ip}:{ipAddress.Port}");
                    }, null, h => { h.UseProxy = false; h.Proxy = null; })));

                }
            }
            return result;
        }

        public async ValueTask Check()
        {
            foreach (var address in _config.Addresses)
            {
                if (!await _healthCheckService.IsHealth(address))
                {
                    throw new Exception(string.Format("注册中心{0}连接异常，请联系管理员", address.ToString()));
                }
            }
        }
    }
}
