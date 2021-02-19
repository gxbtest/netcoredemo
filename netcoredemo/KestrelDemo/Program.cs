
using BimTech.Core.Common.Config;
using BimTech.Core.Common.Extensions;
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

namespace aa
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                //111
                var host = new WebHostBuilder()
                    .Configure()
                    .ConfigureSurging()
                .UseUrls("http://"+new IPEndPoint(IPAddress.Parse(SurgingConfig.MicroService.Surging.Ip), SurgingConfig.MicroService.Surging.Ports.HttpPort).ToString())
                 .UseKestrel((context, options) =>
                 {
                     options.Limits.MinRequestBodyDataRate = null;
                     options.Limits.MinResponseDataRate = null;
                     options.Limits.MaxRequestBodySize = null;
                     options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(30);
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
