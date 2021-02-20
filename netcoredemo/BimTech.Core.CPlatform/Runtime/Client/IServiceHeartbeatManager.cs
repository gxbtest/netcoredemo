using System;
using System.Collections.Generic;
using System.Text;

namespace BimTech.Core.CPlatform.Runtime.Client
{
    public interface IServiceHeartbeatManager
    {
        void AddWhitelist(string serviceId);

        bool ExistsWhitelist(string serviceId);
    }
}
