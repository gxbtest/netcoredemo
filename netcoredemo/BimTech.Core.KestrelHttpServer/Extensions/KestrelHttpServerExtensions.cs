using BimTech.Core.CPlatform.Routing;
using BimTech.Core.CPlatform.Runtime.Server;
using BimTech.Core.CPlatform.Serialization;
using BimTech.Core.KestrelHttpServer.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BimTech.Core.KestrelHttpServer.Extensions
{
    public static class KestrelHttpServerExtensions
    {
        public static void AppResolve(this IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                var messageId = Guid.NewGuid().ToString("N");
                var _serializer = (ISerializer<string>)app.ApplicationServices.GetService(typeof(ISerializer<string>));
                var serviceRouteProvider = (IServiceRouteProvider)app.ApplicationServices.GetService(typeof(IServiceRouteProvider));
                var serviceExecutor = (IServiceExecutor)app.ApplicationServices.GetService(typeof(IServiceExecutor));
                var sender = new HttpServerMessageSender(_serializer, context, new DiagnosticListener("BimTechDiagnosticListener"));
                HttpMessageListener httpMessageListener = new HttpMessageListener(_serializer, serviceRouteProvider, serviceExecutor);
                var actionFilters = app.ApplicationServices.GetServices<IActionFilter>();
                await httpMessageListener.OnReceived(sender, messageId, context, actionFilters);
                //await context.Response.WriteAsync("123");
            });
        }
    }
}
