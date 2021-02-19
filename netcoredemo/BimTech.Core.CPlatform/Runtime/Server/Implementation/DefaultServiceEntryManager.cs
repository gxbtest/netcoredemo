﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BimTech.Core.CPlatform.Runtime.Server.Implementation
{
    /// <summary>
    /// 默认的服务条目管理者。
    /// </summary>
    public class DefaultServiceEntryManager : IServiceEntryManager
    {
        private IEnumerable<ServiceEntry> _serviceEntries;

        private IEnumerable<ServiceEntry> _allEntries;
        public DefaultServiceEntryManager(IEnumerable<IServiceEntryProvider> providers)
        {
            UpdateEntries(providers);
        }

        public IEnumerable<ServiceEntry> GetAllEntries()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ServiceEntry> GetEntries()
        {
            throw new NotImplementedException();
        }

        public void UpdateEntries(IEnumerable<IServiceEntryProvider> providers)
        {
            var list = new List<ServiceEntry>();
            var allEntries = new List<ServiceEntry>();
            foreach (var provider in providers)
            {
                var entries = provider.GetEntries().ToArray();
                foreach (var entry in entries)
                {
                    if (list.Any(i => i.Descriptor.Id == entry.Descriptor.Id))
                        throw new InvalidOperationException($"本地包含多个Id为：{entry.Descriptor.Id} 的服务条目。");
                }
                list.AddRange(entries);
                allEntries.AddRange(provider.GetALLEntries());
            }
            _serviceEntries = list.ToArray();
            _allEntries = allEntries;
        }
    }
}
