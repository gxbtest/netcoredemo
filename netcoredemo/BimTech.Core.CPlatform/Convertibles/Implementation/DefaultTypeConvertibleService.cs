using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BimTech.Core.CPlatform.Convertibles.Implementation
{
    /// <summary>
    /// 一个默认的类型转换服务。
    /// </summary>
    public class DefaultTypeConvertibleService : ITypeConvertibleService
    {
        #region Field

        private readonly IEnumerable<TypeConvertDelegate> _converters;

        #endregion Field

        #region Constructor

        public DefaultTypeConvertibleService(IEnumerable<ITypeConvertibleProvider> providers)
        {
            providers = providers.ToArray();
            _converters = providers.SelectMany(p => p.GetConverters()).ToArray();
        }

        #endregion Constructor

        #region Implementation of ITypeConvertibleService

        /// <summary>
        /// 转换。
        /// </summary>
        /// <param name="instance">需要转换的实例。</param>
        /// <param name="conversionType">转换的类型。</param>
        /// <returns>转换之后的类型，如果无法转换则返回null。</returns>
        public object Convert(object instance, Type conversionType)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (conversionType == null)
                throw new ArgumentNullException(nameof(conversionType));

            if (conversionType.GetTypeInfo().IsInstanceOfType(instance))
                return instance;

            object result = null;
            foreach (var converter in _converters)
            {
                result = converter(instance, conversionType);
                if (result != null)
                    break;
            }
            if (result != null)
                return result;
            Exception exception = new Exception($"将 {instance.GetType()} 转换成 {conversionType} 时发生了错误。");
            throw exception;
        }

        #endregion Implementation of ITypeConvertibleService
    }
}
