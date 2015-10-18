using System;
using System.Linq;
using System.Management.Instrumentation;
using Microphone.Nancy;

namespace Service1
{
    class Program
    {
        private static void Main(string[] args)
        {
            Bootstrap.Start("Service1","v1");           
            Console.ReadLine();
        }
    }

    public class MyService : AutoRegisterModule
    {
        public MyService()
        {
            Get["/"] = _ =>
            {
                //var instances = FindService("Service2");
                //var instance = instances.First(); //or use random index for load balancing

                //MakeSomeCall("/api/orders",instance.ServiceAddress, instance.ServicePort);

                return "Hello";
            };            
        }
    }
}