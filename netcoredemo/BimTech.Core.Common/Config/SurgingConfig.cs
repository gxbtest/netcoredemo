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

        public ConsulOption Consul { get; set; }

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

        public int Port { get; set; }

        public int Weight { get; set; }

        public string WanIp { get; set; }

        public string MappingIP { get; set; }

        public int MappingPort { get; set; }

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

    public class ConsulOption
    {
        public string SessionTimeout { get; set; }

        public string ConnectionString { get; set; }

        public string RoutePath { get; set; }

        public string SubscriberPath { get; set; }

        public string CommandPath { get; set; }

        public string CachePath { get; set; }

        public string MqttRoutePath { get; set; }

        public string ReloadOnChange { get; set; }

        public string EnableChildrenMonitor { get; set; }

        public int? LockDelay { get; set; }
    }

}
