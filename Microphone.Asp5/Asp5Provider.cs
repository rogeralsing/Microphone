using System;
using Microphone.Core;

namespace Microphone.Asp5
{
    public class Asp5Provider : IFrameworkProvider
    {
        public Asp5Provider()
        {
        }

        public Uri Start(string serviceName, string version)
        {
            var uri = Configuration.GetUri();
    //        var config = new HttpSelfHostConfiguration(uri);

    //        config.Routes.MapHttpRoute(
    //            "API Default", "{controller}/{id}",
    //            new { id = RouteParameter.Optional });

    //        var server = new HttpSelfHostServer(config);
    //        server.OpenAsync().Wait();
    //        return uri;
            return new Uri("");
        }
    }
}
