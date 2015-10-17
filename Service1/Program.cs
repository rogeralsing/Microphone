using System;
using Microphone.Nancy;

namespace Service1
{
    class Program
    {
        private static void Main(string[] args)
        {
            Bootstrap.Start("MyService","v1");           
            Console.ReadLine();
        }
    }

    public class MyService : AutoRegisterModule
    {
        public MyService()
        {
            Get["/"] = _ =>
            {
                var instances = FindService("OtherService");

                return "Hello";
            };            
        }
    }
}