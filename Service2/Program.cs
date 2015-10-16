using System;
using NancyBotstrapper;

namespace Service2
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrap.Start("OtherService","v1");
            Console.ReadLine();
        }
    }

    public class OtherService : AutoRegisterModule
    {
        public OtherService()
        {
            Get["/"] = _ => "Other Service!!!";
        }
    }
}
