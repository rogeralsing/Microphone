using System;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Microphone.Core;

namespace Microphone.WebApi
{
    public class WebApiProvider : IWebFrameworkProvider
    {
        public Uri Start(string serviceName, string version)
        {
            var uri = Configuration.GetUri();
            var config = new HttpSelfHostConfiguration(uri);

            config.Routes.MapHttpRoute(
                "API Default", "{controller}/{id}",
                new { id = RouteParameter.Optional });

            var server = new HttpSelfHostServer(config);
            server.OpenAsync().Wait();
            return uri;
        }
    }
}
