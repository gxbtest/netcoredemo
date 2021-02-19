using System;
using System.Collections.Generic;
using System.Text;

namespace BimTech.Core.Common.Config
{
    public class SurgingConfig
    {
        private static MicroService microService = new MicroService();
        public static MicroService MicroService
        {
            get
            {
                return microService;
            }
            set
            {
                MicroService = microService;
            }
        }
    }

    /// <summary>
    /// 配置静态类
    /// </summary>
    public class MicroService
    {
        public Surging Surging { get; set; }

        public Consul Consul { get; set; }

    }

    /// <summary>
    /// 框架统一配置
    /// </summary>
    public class Surging
    {
        /// <summary>
        /// 启动Ip
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 各种协议端口号
        /// </summary>
        public Ports Ports { get; set; }
    }

    public class Ports
    {
        public int HttpPort { get; set; }

        public int WSPort { get; set; }

        public int MQTTPort { get; set; }

        public int GrpcPort { get; set; }
    }

    public class Consul
    {
        /// <summary>
        /// consul连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        public int SessionTimeout { get; set; }

        public string RoutePath { get; set; }

        public bool ReloadOnChange { get; set; }

        public bool EnableChildrenMonitor { get; set; }
    }

}
