using BimTech.Core.CPlatform.Messages;
using BimTech.Core.CPlatform.Routing;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BimTech.Core.KestrelHttpServer.Filters.Implementation
{
    public class AuthorizationFilterContext
    {
        public ServiceRoute Route { get; internal set; }

        public string Path { get; set; }

        public HttpResultMessage<object> Result { get; set; }

        public HttpContext Context { get; internal set; }
    }
}
