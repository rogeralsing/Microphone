using System;
using Microphone.Core;
using Microphone.Core.ClusterProviders;
using Microphone.Nancy;
using Nancy;

namespace Service1
{
    class Program
    {
        private static void Main(string[] args)
        {
            Cluster.Bootstrap<NancyProvider,ConsulProvider>("Service1","v1");           
            Console.ReadLine();
        }
    }

    public class MyService : NancyModule
    {
        public MyService()
        {
            Get["/"] = _ =>
            {
                var res = Cluster.FindService("Service2");                
                return "Hello";
            };            
        }
    }
}