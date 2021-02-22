using BimTech.Core.CPlatform.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace BimTech.Core.CPlatform.Filters.Implementation
{
    public class RpcActionExecutedContext
    {

        public RemoteInvokeMessage InvokeMessage { get; set; }
         
        public Exception Exception { get; set; }
    }
}
