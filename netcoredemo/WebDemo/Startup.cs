
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BimTech.Core.Common.Extensions;
using System;
using BimTech.Core.Common.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public IConfigurationBuilder build { get; set; }

        /// <summary>
        /// 配置管道 中间件
        /// </summary>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //build = new ConfigurationBuilder();
            //build.AddConfigFile(AppContext.BaseDirectory + "appsettings.json", false, true);
            //build.Build().Bind(AppConfig.settingsOptions);
            //SettingsOptions settingsOptions = AppConfig.settingsOptions;
            //app.UseStaticFiles();
            //app.UseMvc();
            app.Run(async (context) =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("<!DOCTYPE html><html lang=\"en\"><head><title></title></head><body><p>Hosted by Kestrel</p>");
            });
        }



        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvcCore();
        }
    }
}
