using Consul;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BimTech.Core.Consul.WatcherProvider.Implementation
{
    public abstract class Watcher
    {
        protected Watcher()
        {
        }

        public abstract Task Process();

        public static class Event
        {
            public enum KeeperState
            {
                Disconnected = 0,
                SyncConnected = 3,
            }
        }
    }
}
