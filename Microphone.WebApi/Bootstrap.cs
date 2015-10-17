using System;
using System.Net;
using System.Net.Sockets;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace Microphone.WebApi
{
    public static class Bootstrap
    {
        public static void Start()
        {
            var uri = GetUri();
            var config = new HttpSelfHostConfiguration(uri);

            config.Routes.MapHttpRoute(
                "API Default", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            using (HttpSelfHostServer server = new HttpSelfHostServer(config))
            {
                server.OpenAsync().Wait();
                Console.WriteLine("Press Enter to quit.");
                Console.ReadLine();
            }
        }


        private static Uri GetUri(int port = 0)
        {
            port = port == 0 ? FreeTcpPort() : port;
            var uri = new Uri("http://localhost:" + port);
            return uri;
        }

        private static int FreeTcpPort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}
