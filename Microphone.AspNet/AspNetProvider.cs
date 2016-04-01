using System;
using Microphone.Core;

namespace Microphone.AspNet
{
    public class AspNetProvider : IFrameworkProvider
    {
        private readonly int _port;

        public AspNetProvider(int port)
        {
            _port = port;
        }

        public Uri Start(string serviceName, string version)
        {
            return new Uri("http://localhost:" + _port);
        }
    }
}