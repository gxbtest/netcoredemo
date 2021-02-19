using BimTech.Core.Common.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;

namespace BimTech.Core.Common.Extensions
{
   public static class ConfigurationExtensions
    {

        public static IConfigurationBuilder AddConfigFile(this IConfigurationBuilder build, string path, bool optional=false, bool reloadOnChange=true)
        {
            build.AddJsonFile(path, optional, reloadOnChange);
            return build;
        }

        /// <summary>
        /// 新增appsettings配置信息
        /// </summary>
        /// <param name="webHostBuilder"></param>
        /// <returns></returns>
        public static WebHostBuilder Configure(this WebHostBuilder webHostBuilder)
        {
           var build = new ConfigurationBuilder();
            build.AddConfigFile(AppContext.BaseDirectory + "appsettings.json", false, true);
            build.Build().Bind(AppConfig.settingsOptions);
            return webHostBuilder;
        }

        /// <summary>
        /// 新增surgingSettings配置信息
        /// </summary>
        /// <param name="webHostBuilder"></param>
        /// <returns></returns>
        public static WebHostBuilder ConfigureSurging(this WebHostBuilder webHostBuilder)
        {
            var build = new ConfigurationBuilder();
            build.AddConfigFile(AppContext.BaseDirectory + "surgingSettings.json", false, true);
            build.Build().Bind(SurgingConfig.MicroService);
            return webHostBuilder;
        }

    }
}
