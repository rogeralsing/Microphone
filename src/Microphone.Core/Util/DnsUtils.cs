using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Microphone.Util
{
    public class DnsUtils
    {
        public static string GetLocalIPAddress()
            => GetLocalIPAddress(Dns.GetHostName());

        public static string GetLocalIPAddress(Uri uri)
            => GetLocalIPAddress(uri.Host);

        public static string GetLocalIPAddress(string hostName)
        {
            var host = Dns.GetHostEntryAsync(hostName).Result;
            foreach (var ip in host.AddressList.OrderBy(ip => ip.ToString()))
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
    }
}