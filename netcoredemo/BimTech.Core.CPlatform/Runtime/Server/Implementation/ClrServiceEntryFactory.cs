﻿using BimTech.Core.CPlatform.Attributes;
using BimTech.Core.CPlatform.Ids;
using BimTech.Core.CPlatform.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BimTech.Core.CPlatform.Runtime.Server.Implementation
{
    /// <summary>
    /// Clr服务条目工厂。
    /// </summary>
    public class ClrServiceEntryFactory : IClrServiceEntryFactory
    {
        private readonly IServiceIdGenerator _serviceIdGenerator;
        public ClrServiceEntryFactory(IServiceIdGenerator serviceIdGenerator)
        {
            _serviceIdGenerator = serviceIdGenerator;
        }
        /// <summary>
        /// 创建服务条目。
        /// </summary>
        /// <param name="service">服务类型。</param>
        /// <param name="serviceImplementation">服务实现类型。</param>
        /// <returns>服务条目集合。</returns>
        public IEnumerable<ServiceEntry> CreateServiceEntry(Type service)
        {
            var routeTemplate = service.GetCustomAttribute<ServiceBundleAttribute>();
            foreach (var methodInfo in service.GetTypeInfo().GetMethods())
            {
                var serviceRoute = methodInfo.GetCustomAttribute<ServiceRouteAttribute>();
                var routeTemplateVal = routeTemplate.RouteTemplate;
                if (!routeTemplate.IsPrefix && serviceRoute != null)
                    routeTemplateVal = serviceRoute.Template;
                else if (routeTemplate.IsPrefix && serviceRoute != null)
                    routeTemplateVal = $"{ routeTemplate.RouteTemplate}/{ serviceRoute.Template}";
                yield return Create(methodInfo, service.Name, routeTemplateVal);
            }
        }

        private ServiceEntry Create(MethodInfo method, string serviceName, string routeTemplate)
        {
            var serviceId = _serviceIdGenerator.GenerateServiceId(method);
            var attributes = method.GetCustomAttributes().ToList();
            var serviceDescriptor = new ServiceDescriptor
            {
                Id = serviceId,
                RoutePath = RoutePatternParser.Parse(routeTemplate, serviceName, method.Name)
            };
            var descriptorAttributes = method.GetCustomAttributes<ServiceDescriptorAttribute>();
            foreach (var descriptorAttribute in descriptorAttributes)
            {
                descriptorAttribute.Apply(serviceDescriptor);
            }
            //var httpMethodAttributes = attributes.Where(p => p is HttpMethodAttribute).Select(p => p as HttpMethodAttribute).ToList();
            //var httpMethods = new List<string>();
            //StringBuilder httpMethod = new StringBuilder();
            //foreach (var attribute in httpMethodAttributes)
            //{
            //    httpMethods.AddRange(attribute.HttpMethods);
            //    if (attribute.IsRegisterMetadata)
            //        httpMethod.AppendJoin(',', attribute.HttpMethods).Append(",");
            //}
            //if (httpMethod.Length > 0)
            //{
            //    httpMethod.Length = httpMethod.Length - 1;
            //    serviceDescriptor.HttpMethod(httpMethod.ToString());
            //}
            //var authorization = attributes.Where(p => p is AuthorizationFilterAttribute).FirstOrDefault();
            //if (authorization != null)
            //    serviceDescriptor.EnableAuthorization(true);
            //if (authorization != null)
            //{
            //    serviceDescriptor.AuthType(((authorization as AuthorizationAttribute)?.AuthType)
            //        ?? AuthorizationType.AppSecret);
            //}
            //var fastInvoker = GetHandler(serviceId, method);

            //var methodValidateAttribute = attributes.Where(p => p is ValidateAttribute)
            //    .Cast<ValidateAttribute>().FirstOrDefault();

            return new ServiceEntry
            {
                Descriptor = serviceDescriptor,
                RoutePath = serviceDescriptor.RoutePath,
                //Methods = httpMethods,
                MethodName = method.Name,
                Type = method.DeclaringType,
                Attributes = attributes,
                //Func = (key, parameters) =>
                //{
                //    object instance = null;
                //    //if (AppConfig.ServerOptions.IsModulePerLifetimeScope)
                //    //    instance = _serviceProvider.GetInstancePerLifetimeScope(key, method.DeclaringType);
                //    //else
                //    //    instance = _serviceProvider.GetInstances(key, method.DeclaringType);
                //    var list = new List<object>();

                //    foreach (var parameterInfo in method.GetParameters())
                //    {
                //        //加入是否有默认值的判断，有默认值，并且用户没传，取默认值
                //        if (parameterInfo.HasDefaultValue && !parameters.ContainsKey(parameterInfo.Name))
                //        {
                //            list.Add(parameterInfo.DefaultValue);
                //            continue;
                //        }
                //        else if (parameterInfo.ParameterType == typeof(CancellationToken))
                //        {
                //            list.Add(new CancellationToken());
                //            continue;
                //        }
                //        var value = parameters[parameterInfo.Name];

                //        if (methodValidateAttribute != null)
                //            _validationProcessor.Validate(parameterInfo, value);

                //        var parameterType = parameterInfo.ParameterType;
                //        var parameter = _typeConvertibleService.Convert(value, parameterType);
                //        list.Add(parameter);
                //    }
                //    var result = fastInvoker(instance, list.ToArray());
                //    return Task.FromResult(result);
                //}
            };
        }
    }
}