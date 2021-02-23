using BimTech.Core.Common.Config;
using BimTech.Core.Consul;
using BimTech.Core.Consul.Configurations;
using BimTech.Core.Consul.Internal;
using BimTech.Core.Consul.Internal.Cluster.HealthChecks;
using BimTech.Core.Consul.Internal.Cluster.HealthChecks.Implementation;
using BimTech.Core.Consul.Internal.Cluster.Implementation.Selectors;
using BimTech.Core.Consul.Internal.Cluster.Implementation.Selectors.Implementation;
using BimTech.Core.Consul.Internal.Implementation;
using BimTech.Core.Consul.WatcherProvider;
using BimTech.Core.Consul.WatcherProvider.Implementation;
using BimTech.Core.CPlatform.Convertibles;
using BimTech.Core.CPlatform.Convertibles.Implementation;
using BimTech.Core.CPlatform.Filters;
using BimTech.Core.CPlatform.Filters.Implementation;
using BimTech.Core.CPlatform.Ids;
using BimTech.Core.CPlatform.Ids.Implementation;
using BimTech.Core.CPlatform.Routing;
using BimTech.Core.CPlatform.Routing.Implementation;
using BimTech.Core.CPlatform.Runtime.Client;
using BimTech.Core.CPlatform.Runtime.Client.Implementation;
using BimTech.Core.CPlatform.Runtime.Server;
using BimTech.Core.CPlatform.Runtime.Server.Implementation;
using BimTech.Core.CPlatform.Serialization;
using BimTech.Core.CPlatform.Serialization.Implementation;
using BimTech.Core.CPlatform.Transport;
using BimTech.Core.CPlatform.Validation;
using BimTech.Core.CPlatform.Validation.Implementation;
using BimTech.Core.KestrelHttpServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using BimTech.Core.CPlatform;
using BimTech.Core.CPlatform.Utilities;
using Autofac;
using BimTech.Core.CPlatform.Ioc;

namespace BimTech.Core.ServiceHosting.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static List<Assembly> _referenceAssembly = new List<Assembly>();

        public static IServiceCollection AddRuntime(this IServiceCollection services)
        {
            var assemblys = GetReferenceAssembly();
            var types = assemblys.SelectMany(i => i.ExportedTypes).ToArray();
          
            ServiceTokenGenerator serviceTokenGenerator = new ServiceTokenGenerator();
            serviceTokenGenerator.GeneratorToken("True");
            services.AddSingleton(typeof(CPlatformContainer), new CPlatformContainer(ServiceLocator.Current));
            //注册服务token生成接口 
            services.AddSingleton(typeof(IServiceTokenGenerator), serviceTokenGenerator);
            //注册服务器路由接口 
            services.AddSingleton<IServiceRouteProvider, DefaultServiceRouteProvider>();
            //注册服务ID生成实例 
            services.AddSingleton<IServiceIdGenerator, DefaultServiceIdGenerator>();
            services.AddSingleton<ITypeConvertibleService, DefaultTypeConvertibleService>();
            services.AddSingleton<IValidationProcessor, DefaultValidationProcessor>();
            var provider = services.BuildServiceProvider();
            var clrServiceEntryFactory = new ClrServiceEntryFactory(provider.GetService<CPlatformContainer>(), provider.GetService<IServiceIdGenerator>(), provider.GetService<ITypeConvertibleService>(), provider.GetService<IValidationProcessor>());
            services.AddSingleton(typeof(IClrServiceEntryFactory), clrServiceEntryFactory);
            provider = services.BuildServiceProvider();
            //services.AddSingleton<IClrServiceEntryFactory, ClrServiceEntryFactory>();
            var attributeServiceEntryProvider=  new AttributeServiceEntryProvider(types, provider.GetService<IClrServiceEntryFactory>());
            services.AddSingleton(typeof(IServiceEntryProvider), attributeServiceEntryProvider);
            services.AddSingleton<IServiceEntryManager, DefaultServiceEntryManager>();
            services.AddSingleton<IServiceEntryLocate, DefaultServiceEntryLocate>();
           
            services.AddSingleton<IAuthorizationFilter, AuthorizationAttribute>();
            services.AddSingleton<IFilter, AuthorizationAttribute>();
            services.AddSingleton<IServiceExecutor, HttpExecutor>();
            services.AddSingleton<IMessageListener, HttpMessageListener>();
            return services;
        }

        public static IServiceCollection AddCoreService(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var routeProvider = provider.GetService<IServiceRouteProvider>();
            routeProvider.RegisterRoutes(0);
            return services;
        }

        public static IServiceCollection AddConsulService(this IServiceCollection services)
        {
            //注册服务路由工厂 
            services.AddSingleton<IServiceRouteFactory, DefaultServiceRouteFactory>();
            services.AddSingleton<IServiceRouteManager, ConsulServiceRouteManager>();
            services.AddSingleton<IHealthCheckService, DefaultHealthCheckService>();
            services.AddSingleton<IConsulAddressSelector, ConsulRandomAddressSelector>();
            services.AddSingleton<IServiceHeartbeatManager, DefaultServiceHeartbeatManager>();
            services.AddSingleton<ISerializer<string>, JsonSerializer>();
            services.AddSingleton<ISerializer<byte[]>, StringByteArraySerializer>();
            services.AddSingleton<ISerializer<object>, StringObjectSerializer>();
            var provider = services.BuildServiceProvider();
            var configInfo = new ConfigInfo(null);
            var config = GetConfigInfo(configInfo);
          var defaultConsulClientProvider= new DefaultConsulClientProvider(config, provider.GetService<IHealthCheckService>(), provider.GetService<IConsulAddressSelector>());
         
            services.AddSingleton(typeof(IConsulClientProvider), defaultConsulClientProvider);
            var clientWatchManager = new ClientWatchManager(config);
            services.AddSingleton(typeof(IClientWatchManager), clientWatchManager);
            provider = services.BuildServiceProvider();
            var consulServiceRouteManager = new ConsulServiceRouteManager(config, provider.GetService<ISerializer<byte[]>>(), provider.GetService<ISerializer<string>>(), provider.GetService<IClientWatchManager>(), provider.GetService<IServiceRouteFactory>(), provider.GetService<IServiceHeartbeatManager>(), provider.GetService<IConsulClientProvider>());
            services.AddSingleton(typeof(IServiceRouteManager), consulServiceRouteManager);
            return services;
        }

        private static ConfigInfo GetConfigInfo(ConfigInfo config)
        {
            ConsulOption option = SurgingConfig.MicroService.Consul;
            if (option != null)
            {
                var sessionTimeout = config.SessionTimeout.TotalSeconds;
                Double.TryParse(option.SessionTimeout, out sessionTimeout);
                config = new ConfigInfo(
                   option.ConnectionString,
                    TimeSpan.FromSeconds(sessionTimeout),
                    option.LockDelay ?? config.LockDelay,
                    option.RoutePath ?? config.RoutePath,
                    option.SubscriberPath ?? config.SubscriberPath,
                    option.CommandPath ?? config.CommandPath,
                    option.CachePath ?? config.CachePath,
                    option.MqttRoutePath ?? config.MqttRoutePath,
                   option.ReloadOnChange != null ? bool.Parse(option.ReloadOnChange) :
                    config.ReloadOnChange,
                    option.EnableChildrenMonitor != null ? bool.Parse(option.EnableChildrenMonitor) :
                    config.EnableChildrenMonitor
                   );
            }
            return config;
        }

        private static List<Assembly> GetAssemblies(params string[] virtualPaths)
        {
            var referenceAssemblies = new List<Assembly>();
            if (virtualPaths.Any())
            {
                referenceAssemblies = GetReferenceAssembly(virtualPaths);
            }
            else
            {
                string[] assemblyNames = DependencyContext
                    .Default.GetDefaultAssemblyNames().Select(p => p.Name).ToArray();
                assemblyNames = GetFilterAssemblies(assemblyNames);
                foreach (var name in assemblyNames)
                    referenceAssemblies.Add(Assembly.Load(name));
                _referenceAssembly.AddRange(referenceAssemblies.Except(_referenceAssembly));
            }
            return referenceAssemblies;
        }

        private static string[] GetFilterAssemblies(string[] assemblyNames)
        {
            var notRelatedFile = "";
            var relatedFile = "";
            var pattern = string.Format("^Microsoft.\\w*|^System.\\w*|^DotNetty.\\w*|^runtime.\\w*|^ZooKeeperNetEx\\w*|^StackExchange.Redis\\w*|^Consul\\w*|^Newtonsoft.Json.\\w*|^Autofac.\\w*{0}",
               string.IsNullOrEmpty(notRelatedFile) ? "" : $"|{notRelatedFile}");
            Regex notRelatedRegex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Regex relatedRegex = new Regex(relatedFile, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (!string.IsNullOrEmpty(relatedFile))
            {
                return
                    assemblyNames.Where(
                        name => !notRelatedRegex.IsMatch(name) && relatedRegex.IsMatch(name)).ToArray();
            }
            else
            {
                return
                    assemblyNames.Where(
                        name => !notRelatedRegex.IsMatch(name)).ToArray();
            }
        }

        private static List<Assembly> GetReferenceAssembly(params string[] virtualPaths)
        {
            var refAssemblies = new List<Assembly>();
            var rootPath = AppContext.BaseDirectory;
            var existsPath = virtualPaths.Any();
            var result = _referenceAssembly;
            if (!result.Any() || existsPath)
            {
                var paths = virtualPaths.Select(m => Path.Combine(rootPath, m)).ToList();
                if (!existsPath) paths.Add(rootPath);
                paths.ForEach(path =>
                {
                    var assemblyFiles = GetAllAssemblyFiles(path);

                    foreach (var referencedAssemblyFile in assemblyFiles)
                    {
                        var referencedAssembly = Assembly.LoadFrom(referencedAssemblyFile);
                        if (!_referenceAssembly.Contains(referencedAssembly))
                            _referenceAssembly.Add(referencedAssembly);
                        refAssemblies.Add(referencedAssembly);
                    }
                    result = existsPath ? refAssemblies : _referenceAssembly;
                });
            }
            return result;
        }

        /// <summary>.
        /// 依赖注入业务模块程序集 接口及接口实现
        /// </summary>
        /// <param name="builder">ioc容器</param>
        /// <returns>返回注册模块信息</returns>
        public static ContainerBuilder RegisterServices(this ContainerBuilder services, params string[] virtualPaths)
        {
            try
            {
                var referenceAssemblies = GetAssemblies(virtualPaths);
                foreach (var assembly in referenceAssemblies)
                {
                    services.RegisterAssemblyTypes(assembly)
                       //注入继承IServiceKey接口的所有接口
                       .Where(t => typeof(IServiceKey).GetTypeInfo().IsAssignableFrom(t) && t.IsInterface)
                       .AsImplementedInterfaces();
                    services.RegisterAssemblyTypes(assembly)
                 //注入实现IServiceBehavior接口并ModuleName为空的类，作为接口实现类
                 .Where(t => !typeof(ISingleInstance).GetTypeInfo().IsAssignableFrom(t) &&
                 typeof(IServiceBehavior).GetTypeInfo().IsAssignableFrom(t) && t.GetTypeInfo().GetCustomAttribute<ModuleNameAttribute>() == null).AsImplementedInterfaces();

                    services.RegisterAssemblyTypes(assembly)
             //注入实现IServiceBehavior接口并ModuleName为空的类，作为接口实现类
             .Where(t => typeof(ISingleInstance).GetTypeInfo().IsAssignableFrom(t) &&
             typeof(IServiceBehavior).GetTypeInfo().IsAssignableFrom(t) && t.GetTypeInfo().GetCustomAttribute<ModuleNameAttribute>() == null).SingleInstance().AsImplementedInterfaces();

                    var types = assembly.GetTypes().Where(t => typeof(IServiceBehavior).GetTypeInfo().IsAssignableFrom(t) && t.GetTypeInfo().GetCustomAttribute<ModuleNameAttribute>() != null);
                    foreach (var type in types)
                    {
                        var module = type.GetTypeInfo().GetCustomAttribute<ModuleNameAttribute>();
                        //对ModuleName不为空的对象，找到第一个继承IServiceKey的接口并注入接口及实现
                        var interfaceObj = type.GetInterfaces()
                            .FirstOrDefault(t => typeof(IServiceKey).GetTypeInfo().IsAssignableFrom(t));
                        if (interfaceObj != null)
                        {
                            services.RegisterType(type).AsImplementedInterfaces().Named(module.ModuleName, interfaceObj);
                            services.RegisterType(type).Named(module.ModuleName, type);
                        }
                    }

                }
                return services;
            }
            catch (Exception ex)
            {
                if (ex is System.Reflection.ReflectionTypeLoadException)
                {
                    var typeLoadException = ex as ReflectionTypeLoadException;
                    var loaderExceptions = typeLoadException.LoaderExceptions;
                    throw loaderExceptions[0];
                }
                throw ex;
            }
        }

        private static List<string> GetAllAssemblyFiles(string parentDir)
        {
            var notRelatedFile = "";
            var relatedFile = "";
            var pattern = string.Format("^Microsoft.\\w*|^System.\\w*|^Netty.\\w*|^Autofac.\\w*{0}",
               string.IsNullOrEmpty(notRelatedFile) ? "" : $"|{notRelatedFile}");
            Regex notRelatedRegex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Regex relatedRegex = new Regex(relatedFile, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (!string.IsNullOrEmpty(relatedFile))
            {
                return
                    Directory.GetFiles(parentDir, "*.dll").Select(Path.GetFullPath).Where(
                        a => !notRelatedRegex.IsMatch(a) && relatedRegex.IsMatch(a)).ToList();
            }
            else
            {
                return
                    Directory.GetFiles(parentDir, "*.dll").Select(Path.GetFullPath).Where(
                        a => !notRelatedRegex.IsMatch(Path.GetFileName(a))).ToList();
            }
        }
    }
}
