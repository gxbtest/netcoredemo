﻿using BimTech.Core.Common.Config;
using BimTech.Core.CPlatform.Address;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace BimTech.Core.CPlatform.Utilities
{
    public class NetUtils
    {
        public const string LOCALHOST = "127.0.0.1";
        public const string ANYHOST = "0.0.0.0";
        private const int MIN_PORT = 0;
        private const int MAX_PORT = 65535;
        private const string LOCAL_IP_PATTERN = "127(\\.\\d{1,3}){3}$";
        private const string IP_PATTERN = "\\d{1,3}(\\.\\d{1,3}){3,5}$";
        private static AddressModel _host = null;

        public static bool IsInvalidPort(int port)
        {
            return port <= MIN_PORT || port > MAX_PORT;
        }

        public static bool IsLocalHost(string host)
        {
            return host != null
                    && (host.IsMatch(LOCAL_IP_PATTERN)
                    || host.Equals("localhost", StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsAnyHost(String host)
        {
            return "0.0.0.0".Equals(host);
        }

        private static bool IsValidAddress(string address)
        {
            return (address != null
                    && !ANYHOST.Equals(address)
                    && address.IsMatch(IP_PATTERN));
        }

        public static bool IsInvalidLocalHost(String host)
        {
            return host == null
                    || host.Length == 0
                    || host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
                    || host.Equals("0.0.0.0")
                    || (host.IsMatch(LOCAL_IP_PATTERN));
        }

        public static string GetAnyHostAddress()
        {
            string result = "";
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    IPInterfaceProperties ipxx = adapter.GetIPProperties();
                    UnicastIPAddressInformationCollection ipCollection = ipxx.UnicastAddresses;
                    foreach (UnicastIPAddressInformation ipadd in ipCollection)
                    {
                        if (ipadd.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            result = ipadd.Address.ToString();
                        }
                    }
                }
            }
            return result;
        }

        public static string GetHostAddress(string hostAddress)
        {
            var result = hostAddress;
            if ((!IsValidAddress(hostAddress) && !IsLocalHost(hostAddress)) || IsAnyHost(hostAddress))
            {
                result = GetAnyHostAddress();
            }
            return result;
        }

        public static AddressModel GetHostAddress()
        {
            if (_host != null)
                return _host;
            var ports = SurgingConfig.MicroService.Surging.Ports;
            string address = GetHostAddress(SurgingConfig.MicroService.Surging.Ip);
            int port = SurgingConfig.MicroService.Surging.Port;
            var mappingIp = SurgingConfig.MicroService.Surging.MappingIP ?? address;
            var mappingPort = SurgingConfig.MicroService.Surging.MappingPort;
            if (mappingPort == 0)
                mappingPort = port;
            _host = new IpAddressModel
            {
                HttpPort = ports.HttpPort,
                Ip = mappingIp,
                Port = mappingPort,
                MqttPort = ports.MQTTPort,
                WanIp = SurgingConfig.MicroService.Surging.WanIp,
                WsPort = ports.WSPort
            };
            return _host;
        }
    }
}
