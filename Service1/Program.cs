using System;
using Microphone.Core;
using Microphone.Nancy;
using Nancy;

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

    public class MyService : NancyModule
    {
        public MyService()
        {
            Get["/"] = _ =>
            {                
                return "Hello";
            };            
        }
    }
}