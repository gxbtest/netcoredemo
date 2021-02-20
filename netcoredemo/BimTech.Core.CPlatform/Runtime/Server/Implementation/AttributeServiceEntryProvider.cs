using BimTech.Core.CPlatform.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BimTech.Core.CPlatform.Runtime.Server.Implementation
{
    /// <summary>
    /// Service标记类型的服务条目提供程序。
    /// </summary>
    public class AttributeServiceEntryProvider : IServiceEntryProvider
    {
        private readonly IEnumerable<Type> _types;
        private readonly IClrServiceEntryFactory _clrServiceEntryFactory;

        public AttributeServiceEntryProvider(IEnumerable<Type> types, IClrServiceEntryFactory clrServiceEntryFactory)
        {
            this._types = types;
            this._clrServiceEntryFactory = clrServiceEntryFactory;
        }

        public IEnumerable<ServiceEntry> GetALLEntries()
        {
            var services = _types.Where(i =>
            {
                var typeInfo = i.GetTypeInfo();
                return typeInfo.IsInterface && typeInfo.GetCustomAttribute<ServiceBundleAttribute>() != null;
            }).Distinct().ToArray();
            //if (_logger.IsEnabled(LogLevel.Information))
            //{
            //    _logger.LogInformation($"发现了以下服务：{string.Join(",", services.Select(i => i.ToString()))}。");
            //}
            var entries = new List<ServiceEntry>();
            foreach (var service in services)
            {
                entries.AddRange(_clrServiceEntryFactory.CreateServiceEntry(service));
            }
            return entries;
        }

        public IEnumerable<ServiceEntry> GetEntries()
        {
            var services = GetTypes();
            var entries = new List<ServiceEntry>();
            foreach (var service in services)
            {
                entries.AddRange(_clrServiceEntryFactory.CreateServiceEntry(service));
            }
            return entries;
        }

        public IEnumerable<Type> GetTypes()
        {
            var services = _types.Where(i =>
            {
                var typeInfo = i.GetTypeInfo();
                return typeInfo.IsInterface && typeInfo.GetCustomAttribute<ServiceBundleAttribute>() != null;
            }).Distinct().ToArray();
            return services;
        }
    }
}
