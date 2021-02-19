using System;

namespace BimTech.Core.CPlatform.Runtime.Server.Implementation
{
    public class ServiceRouteAttribute : Attribute
    {
        public ServiceRouteAttribute(string template)
        {
            Template = template;
        }

        public string Template { get; }
    }
}