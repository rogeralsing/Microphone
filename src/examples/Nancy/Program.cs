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
            container.RegisterMicrophone<ConsulProvider>();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
           new NancyHost(new Uri("http://127.0.0.1:5555")).Start();
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
