
using BimTech.Core.Common.Config;
using BimTech.Core.Common.Extensions;
using BimTech.Core.Common.Help;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Text;

namespace WebDemo
{
    class Program
    {



        public static void Main(string[] args)
        {
            try
            {
                var host = new WebHostBuilder()
                    .Configure()
                .UseUrls(AppConfig.settingsOptions?.appSetting?.Ip)
                 .UseKestrel((context, options) =>
                 {
                     options.Limits.MinRequestBodyDataRate = null;
                     options.Limits.MinResponseDataRate = null;
                     options.Limits.MaxRequestBodySize = null;
                     options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(30);
                     options.Listen(IPAddress.Any, 668 , listenOptions =>
                     {
                         listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                     });
                 })
                 .UseStartup<Startup>()
                .Build();
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
