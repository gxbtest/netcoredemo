
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
using BimTech.Core.KestrelHttpServer.Extensions;
using System.ComponentModel;
using Autofac;
using BimTech.Core.CPlatform.Utilities;

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
        /// 配置管道 中间件
        /// </summary>
        public void Configure(IApplicationBuilder app)
        {
            app.UseStaticFiles();
            //app.UseMvc();
            app.AppResolve();
        }



        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            SetContainer();
            services.AddConsulService();
            services.AddRuntime();
            services.AddCoreService();
        }

        /// <summary>
        /// 配置容器
        /// </summary>
        /// <param name="services"></param>
        public Autofac.IContainer SetContainer()
        {
            var builder = new ContainerBuilder();
            builder =builder.RegisterServices();
            ServiceLocator.Current = builder.Build();
            return ServiceLocator.Current;
        }
    }
}
