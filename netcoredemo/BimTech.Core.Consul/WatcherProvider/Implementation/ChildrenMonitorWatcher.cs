﻿using BimTech.Core.Consul.Extensions;
using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BimTech.Core.Consul.WatcherProvider.Implementation
{
    public class ChildrenMonitorWatcher : WatcherBase
    {
        private readonly Action<string[], string[]> _action;
        private readonly IClientWatchManager _manager;
        private readonly string _path;
        private readonly Func<string[], string[]> _func;
        private string[] _currentData = new string[0];
        private readonly Func<ValueTask<ConsulClient>> _clientCall;
        public ChildrenMonitorWatcher(Func<ValueTask<ConsulClient>> clientCall, IClientWatchManager manager, string path,
            Action<string[], string[]> action, Func<string[], string[]> func)
        {
            this._action = action;
            _manager = manager;
            _clientCall = clientCall;
            _path = path;
            _func = func;
            RegisterWatch();
        }

        public ChildrenMonitorWatcher SetCurrentData(string[] currentData)
        {
            _currentData = currentData ?? new string[0];
            return this;
        }

        protected override async Task ProcessImpl()
        {
            RegisterWatch(this);
            var client = await _clientCall();
            var result = await client.GetChildrenAsync(_path);
            if (result != null)
            {
                var convertResult = _func.Invoke(result).Select(key => $"{_path}{key}").ToArray();
                _action(_currentData, convertResult);
                this.SetCurrentData(convertResult);
            }
        }

        private void RegisterWatch(Watcher watcher = null)
        {
            ChildWatchRegistration wcb = null;
            if (watcher != null)
            {
                wcb = new ChildWatchRegistration(_manager, watcher, _path);
            }
            else
            {
                wcb = new ChildWatchRegistration(_manager, this, _path);
            }
            wcb.Register();
        }
    }
}
