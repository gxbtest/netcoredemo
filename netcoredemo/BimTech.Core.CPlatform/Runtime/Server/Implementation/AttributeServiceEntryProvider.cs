using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
