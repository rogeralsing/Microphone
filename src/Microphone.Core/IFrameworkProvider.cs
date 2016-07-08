using System;

namespace Microphone.Core
{
    public interface IFrameworkProvider
    {
        Uri Start(string serviceName, string version);
    }
}