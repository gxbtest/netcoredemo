using BimTech.Core.Consul.WatcherProvider.Implementation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BimTech.Core.Consul.WatcherProvider
{
    public interface IClientWatchManager
    {
        Dictionary<string, HashSet<Watcher>> DataWatches { get; set; }
    }
}
