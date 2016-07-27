using System;
using Microphone.Consul;
using Microphone.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nancy.TinyIoc;

namespace Microphone.Nancy
{
    public static class TinyIoCContainerExtensions
    {
        public static void RegisterMicrophone<TClusterProvider>(this TinyIoCContainer container,string serviceName,Uri serviceUri, bool useServiceUriHost = false) where TClusterProvider: class,IClusterProvider
        {
            container.Register<IHealthCheck, EmptyHealthCheck>().AsSingleton();
            container.Register<IClusterProvider, TClusterProvider>().AsSingleton();
            container.Register<ILoggerFactory, LoggerFactory>().AsSingleton();
            container.Register<IOptions<ConsulOptions>, OptionsWrapper<ConsulOptions>>().AsSingleton();

            var provider = container.Resolve<IClusterProvider>();
            var loggerFactory = container.Resolve<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("Microphone.Nancy");
            Cluster.RegisterService(serviceUri,provider,serviceName,"1.0",logger, useServiceUriHost);
        }
    }
}
