using System;
using Microphone;

namespace Microphone.Nancy
{
    public class NancyProvider : IFrameworkProvider
    {
        private readonly Uri _uri;

        public NancyProvider(Uri uri)
        {
            _uri = uri;
        }

        public Uri GetUri()
        {
            return _uri;
        }
    }
}
