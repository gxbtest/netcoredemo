using BimTech.Core.CPlatform.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace BimTech.Core.CPlatform.Routing
{
    public class ServiceRouteContext
    {
        public ServiceRoute Route { get; set; }

        public RemoteInvokeResultMessage ResultMessage { get; set; }

        public RemoteInvokeMessage InvokeMessage { get; set; }
    }
}
