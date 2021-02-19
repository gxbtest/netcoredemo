using System;
using System.Collections.Generic;
using System.Text;

namespace BimTech.Core.CPlatform.Attributes
{
    /// <summary>
    /// 服务集标记。
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class ServiceBundleAttribute : Attribute
    {
        public ServiceBundleAttribute(string routeTemplate, bool isPrefix = true)
        {
            RouteTemplate = routeTemplate;
            IsPrefix = isPrefix;
        }
        public string RouteTemplate { get; }

        public bool IsPrefix { get; }
    }
}
