
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
using BimTech.Core.CPlatform.Routing;
using BimTech.Core.ServiceHosting.Extensions;

namespace KestrelDemo
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
        /// ���ùܵ� �м��
        /// </summary>
        public void Configure(IApplicationBuilder app)
        {
            //build = new ConfigurationBuilder();
            //build.AddConfigFile(AppContext.BaseDirectory + "appsettings.json", false, true);
            //build.Build().Bind(AppConfig.settingsOptions);
            //SettingsOptions settingsOptions = AppConfig.settingsOptions;
            //app.UseStaticFiles();
            //app.UseMvc();
           
            app.Run(async (context) =>
            {
                context.Response.Headers.Add("Content-Type", "application/json;charset=utf-8");
                await context.Response.WriteAsync("123");
            });
        }



        /// <summary>
        /// ����ע��
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConsulService();
            services.AddRuntime();
            services.AddCoreService();
          
        }
    }
}
