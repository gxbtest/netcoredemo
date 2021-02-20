﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
            return _allEntries;
        }

        /// <summary>
        /// 获取服务条目集合。
        /// </summary>
        /// <returns>服务条目集合。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<ServiceEntry> GetEntries()
        {
            return _serviceEntries;
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
