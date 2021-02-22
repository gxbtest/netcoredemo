using BimTech.Core.CPlatform.Routing;
using BimTech.Core.CPlatform.Runtime.Client.Address.Resolvers.Implementation.Selectors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BimTech.Core.CPlatform.Filters
{
    public interface IAuthorizationFilter: IFilter
    {
        void ExecuteAuthorizationFilterAsync(ServiceRouteContext serviceRouteContext,CancellationToken cancellationToken);
    }
}
