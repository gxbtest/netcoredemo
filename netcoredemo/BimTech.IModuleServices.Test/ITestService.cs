using BimTech.Core.CPlatform.Attributes;
using BimTech.Core.CPlatform.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BimTech.IModuleServices.Test
{
    [ServiceBundle("api/{Service}")]
    public interface ITestService: IServiceKey
    {
        [Service(Date = "2021-1-23", Director = "gxb", Name = "测试微服务")]
        Task<string> Test();
    }
}
