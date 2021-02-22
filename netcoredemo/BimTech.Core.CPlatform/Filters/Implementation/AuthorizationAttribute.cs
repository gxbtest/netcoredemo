using BimTech.Core.CPlatform.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace BimTech.Core.CPlatform.Filters.Implementation
{
   public  class AuthorizationAttribute : AuthorizationFilterAttribute
    {
        public AuthorizationType AuthType { get; set; }
    }
}
