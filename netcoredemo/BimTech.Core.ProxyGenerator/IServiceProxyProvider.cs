﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BimTech.Core.ProxyGenerator
{
    /// <summary>
    /// 代理服务接口
    /// </summary>
    public interface IServiceProxyProvider
    {
        Task<T> Invoke<T>(IDictionary<string, object> parameters, string routePath);

        Task<T> Invoke<T>(IDictionary<string, object> parameters, string routePath, string serviceKey);
    }
}
