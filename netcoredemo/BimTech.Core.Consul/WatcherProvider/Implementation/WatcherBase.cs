using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BimTech.Core.Consul.WatcherProvider.Implementation
{
    public abstract class WatcherBase : Watcher
    {

        protected WatcherBase()
        {

        }

        public override async Task Process()
        {
            await ProcessImpl();
        }

        protected abstract Task ProcessImpl();
    }
}
