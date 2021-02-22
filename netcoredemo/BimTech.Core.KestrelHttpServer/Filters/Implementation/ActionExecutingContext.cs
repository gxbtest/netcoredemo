using BimTech.Core.CPlatform.Messages;
using BimTech.Core.CPlatform.Routing;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BimTech.Core.KestrelHttpServer.Filters.Implementation
{
    public class ActionExecutingContext
    {
        public HttpMessage Message { get; internal set; }

        public ServiceRoute Route { get; internal set; }

        public HttpResultMessage<object> Result { get; set; }

        public HttpContext Context { get; internal set; }
    }
}
