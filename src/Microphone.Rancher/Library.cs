using System;
using System.Net.Http;

namespace Microphone.Rancher
{
    public static class UriResolver
    {
        public static Uri GetContainerUri(string scheme, string port)
        {
            var host = HttpGet("http://rancher-metadata/2015-12-19/self/container/primary_ip");
            return new Uri($"{scheme}://{host}:{port}");
        }

        public static Uri GetHostPortMapUri(string scheme)
        {
            var port = HttpGet("http://rancher-metadata/2015-12-19/self/service/ports/0").Split(':')[0];
            var host = HttpGet("http://rancher-metadata/2015-12-19/self/host/agent_ip");
            return new Uri($"{scheme}://{host}:{port}");
        }

        private static string HttpGet(string uri)
        {
            var http = new HttpClient();
            http.BaseAddress = new Uri(uri);
            var res = http.GetStringAsync("").Result;
            return res;
        }
    }
}
