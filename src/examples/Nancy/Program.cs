using System;
using Microphone.Consul;
using Microphone.Nancy;
using Nancy;
using Nancy.Hosting.Self;
using Nancy.TinyIoc;

namespace NancyDemo
{
    public class MyBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.RegisterMicrophone<ConsulProvider>("Myservice",new Uri("http://localhost:5555"));
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            HostConfiguration hostConf = new HostConfiguration {RewriteLocalhost = true};
            new NancyHost(hostConf,new Uri("http://localhost:5555")).Start();
            Console.ReadLine();
        }
    }

    public class NancyExample : NancyModule
    {
        public NancyExample()
        {
            Get("/hello", req => "hello");
        }
    }
}
