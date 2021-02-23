using BimTech.Core.ProxyGenerator;
using BimTech.IModuleServices.Test;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BimTech.Modules.Test
{
    public class TestService : ProxyServiceBase, ITestService
    {
        public Task<string> Test()
        {
            return Task<string>.FromResult(Guid.NewGuid().ToString());
        }
    }
}
