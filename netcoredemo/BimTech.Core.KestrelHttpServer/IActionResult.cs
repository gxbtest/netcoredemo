using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BimTech.Core.KestrelHttpServer
{
    public interface IActionResult
    {
        Task ExecuteResultAsync(ActionContext context);
    }
}
