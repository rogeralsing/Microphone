using System;
using Microphone;
using Microphone.Consul;
using Microsoft.Extensions.DependencyInjection;
using Nancy;
using Nancy.Hosting.Self;

namespace NancyDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddSingleton<IClusterProvider, ConsulProvider>();
            services.AddLogging();
            services.AddOptions();
            var ioc = services.BuildServiceProvider();

            ioc.GetService<IClusterProvider>();
            var config = new HostConfiguration();
            var host = new NancyHost(config, new Uri("http://127.0.0.1:5555"));
            host.Start();
            Console.ReadLine();
        }
    }

    public class NancyExample : NancyModule
    {
        public NancyExample()
        {
            Get("/hello", req =>
            {
                return "hello";
            });
        }
    }
}
