using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Hsenl {
    public static class NetworkHelper {
        public static string[] GetAddressIPs() {
            var list = new List<string>();
            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces()) {
                if (networkInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet) {
                    continue;
                }

                foreach (var add in networkInterface.GetIPProperties().UnicastAddresses) {
                    list.Add(add.Address.ToString());
                }
            }

            return list.ToArray();
        }

        // 优先获取IPV4的地址
        public static IPAddress GetHostAddress(string hostName) {
            var ipAddresses = Dns.GetHostAddresses(hostName);
            IPAddress returnIpAddress = null;
            foreach (var ipAddress in ipAddresses) {
                returnIpAddress = ipAddress;
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork) {
                    return ipAddress;
                }
            }

            return returnIpAddress;
        }

        public static IPEndPoint ToIPEndPoint(string host, int port) {
            return new IPEndPoint(IPAddress.Parse(host), port);
        }

        public static IPEndPoint ToIPEndPoint(string address) {
            var index = address.LastIndexOf(':');
            var host = address.Substring(0, index);
            var p = address.Substring(index + 1);
            var port = int.Parse(p);
            return ToIPEndPoint(host, port);
        }
    }
}