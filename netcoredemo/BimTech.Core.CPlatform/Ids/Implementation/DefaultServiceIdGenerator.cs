using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BimTech.Core.CPlatform.Ids.Implementation
{
    /// <summary>
    /// 一个默认的服务Id生成器。
    /// </summary>
    public class DefaultServiceIdGenerator : IServiceIdGenerator
    {
        public DefaultServiceIdGenerator()
        {

        }
        public string GenerateServiceId(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            var type = method.DeclaringType;
            if (type == null)
                throw new ArgumentNullException(nameof(method.DeclaringType), "方法的定义类型不能为空。");
            var id = $"{type.FullName}.{method.Name}";
            var parameters = method.GetParameters();
            if (parameters.Any())
            {
                id += "_" + string.Join("_", parameters.Select(i => i.Name));
            }
            return id;
        }
    }
}
