using BimTech.Core.KestrelHttpServer.Filters.Implementation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BimTech.Core.KestrelHttpServer.Filters
{
    public interface IActionFilter
    {
        Task OnActionExecuting(ActionExecutingContext filterContext);

        Task OnActionExecuted(ActionExecutedContext filterContext);
    }
}
