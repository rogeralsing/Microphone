using System;
using Microphone.Core;
using Microphone.Core.ClusterProviders;
using Microphone.Etcd;
using Microphone.Nancy;
using Nancy;

namespace NancyFxServiceExample
{
    class Program
    {
        private static void Main(string[] args)
        {
            Cluster.Bootstrap<NancyProvider, EtcdProvider>("NancyFxServiceExample", "v1");
            Console.ReadLine();
            var res = Cluster.FindServiceAsync("WebApiServiceExample").Result;
            foreach (var instance in res)
            {
                Console.WriteLine("{0} {1}", instance.ServiceAddress, instance.ServicePort);
            }
            Console.ReadLine();
        }
    }

    public class MyService : NancyModule
    {
        public MyService()
        {
            Get["/"] = _ => "Hello";
        }
    }
}