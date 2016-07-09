using System;
using System.Net;
using System.Net.Sockets;

namespace Microphone.Core.Util
{
    public class DnsUtils
    {
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntryAsync(Dns.GetHostName()).Result;
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        public static string GetLocalEscapedIPAddress()
        {
            return GetLocalIPAddress().Replace(".", "_");
        }
    }
}