using BimTech.Core.CPlatform.Messages;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BimTech.Core.KestrelHttpServer.Filters.Implementation
{
    public class ActionExecutedContext
    {
        public HttpMessage Message { get; internal set; }
        public HttpContext Context { get; internal set; }
    }
}
