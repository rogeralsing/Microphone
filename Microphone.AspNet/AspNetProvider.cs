using System;
using Microphone.Core;

namespace Microphone.AspNet
{
    public class AspNetProvider : IFrameworkProvider
    {
        public AspNetProvider()
        {
        }

        public Uri Start(string serviceName, string version)
        {
            var uri = Configuration.GetUri();
            return new Uri("http://localhost:5001");
        }
    }
}
