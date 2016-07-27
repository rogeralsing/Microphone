using System;
using System.Net;
using System.Net.Sockets;

namespace Microphone.Core.Util
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
            foreach (var ip in host.AddressList)
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