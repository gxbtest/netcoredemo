using BimTech.Core.CPlatform.Messages;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BimTech.Core.KestrelHttpServer
{
    public class ActionContext
    {
        public ActionContext()
        {

        }

        public HttpContext HttpContext { get; set; }

        public TransportMessage Message { get; set; }

    }
}
