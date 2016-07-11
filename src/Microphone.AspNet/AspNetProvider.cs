using System;

namespace Microphone.AspNet
{
    public class AspNetProvider : IFrameworkProvider
    {
        private readonly Uri _uri;

        public AspNetProvider(Uri uri)
        {
            _uri = uri;
        }

        public Uri GetUri()
        {
            return _uri;
        }
    }
}
