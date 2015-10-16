using System;
using NancyBotstrapper;

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
            Get["/"] = _ => "Hello";            
        }
    }
}