using BimTech.Core.Common.Config;
using BimTech.Core.CPlatform.Runtime.Server;
using BimTech.Core.CPlatform.Transport.Implementation;
using BimTech.Core.CPlatform.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BimTech.Core.CPlatform.Routing.Implementation
{
    public class DefaultServiceRouteProvider : IServiceRouteProvider
    {
        private readonly ConcurrentDictionary<string, ServiceRoute> _concurrent =
new ConcurrentDictionary<string, ServiceRoute>();

        private readonly List<ServiceRoute> _localRoutes = new List<ServiceRoute>();

        private readonly ConcurrentDictionary<string, ServiceRoute> _serviceRoute =
       new ConcurrentDictionary<string, ServiceRoute>();
        private readonly IServiceEntryManager _serviceEntryManager;
        private readonly IServiceRouteManager _serviceRouteManager;
        private readonly IServiceTokenGenerator _serviceTokenGenerator;

        public DefaultServiceRouteProvider(IServiceRouteManager serviceRouteManager,
            IServiceEntryManager serviceEntryManager, IServiceTokenGenerator serviceTokenGenerator)
        {
            serviceRouteManager.Changed += ServiceRouteManager_Removed;
            serviceRouteManager.Removed += ServiceRouteManager_Removed;
            serviceRouteManager.Created += ServiceRouteManager_Add;
            _serviceEntryManager = serviceEntryManager;
            _serviceTokenGenerator = serviceTokenGenerator;
            _serviceRouteManager = serviceRouteManager;
        }

        public ValueTask<ServiceRoute> GetLocalRouteByPathRegex(string path)
        {
            throw new NotImplementedException();
        }

        public ValueTask<ServiceRoute> GetRouteByPath(string path)
        {
            throw new NotImplementedException();
        }

        public ValueTask<ServiceRoute> GetRouteByPathRegex(string path)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceRoute> Locate(string serviceId)
        {
            throw new NotImplementedException();
        }

        public async Task RegisterRoutes(decimal processorTime)
        {
            var addess = NetUtils.GetHostAddress();
            addess.ProcessorTime = processorTime;
            addess.Weight = SurgingConfig.MicroService.Surging.Weight;
            if (addess.Weight > 0)
                addess.Timestamp = DateTimeConverter.DateTimeToUnixTimestamp(DateTime.Now);
            RpcContext.GetContext().SetAttachment("Host", addess);
            var addressDescriptors = _serviceEntryManager.GetEntries().Select(i =>
            {
                i.Descriptor.Token = _serviceTokenGenerator.GetToken();
                return new ServiceRoute
                {
                    Address = new[] { addess },
                    ServiceDescriptor = i.Descriptor
                };
            }).ToList();
            await _serviceRouteManager.SetRoutesAsync(addressDescriptors);
        }

        public Task<ServiceRoute> SearchRoute(string path)
        {
            throw new NotImplementedException();
        }

        #region 私有方法
        private static string GetCacheKey(ServiceDescriptor descriptor)
        {
            return descriptor.Id;
        }

        private void ServiceRouteManager_Removed(object sender, ServiceRouteEventArgs e)
        {
            var key = GetCacheKey(e.Route.ServiceDescriptor);
            ServiceRoute value;
            _concurrent.TryRemove(key, out value);
            _serviceRoute.TryRemove(e.Route.ServiceDescriptor.RoutePath, out value);
        }

        private void ServiceRouteManager_Add(object sender, ServiceRouteEventArgs e)
        {
            var key = GetCacheKey(e.Route.ServiceDescriptor);
            _concurrent.GetOrAdd(key, e.Route);
            _serviceRoute.GetOrAdd(e.Route.ServiceDescriptor.RoutePath, e.Route);
        }

       

        #endregion
    }
}
