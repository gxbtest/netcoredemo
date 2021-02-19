using System;
using System.Collections.Generic;
using System.Text;

namespace BimTech.Core.CPlatform.Attributes
{
    public abstract class ServiceDescriptorAttribute:Attribute
    {
        /// <summary>
        /// 应用标记。
        /// </summary>
        /// <param name="descriptor">服务描述符。</param>
        public abstract void Apply(ServiceDescriptor descriptor);
    }
}
