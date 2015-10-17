using System;
using System.Web.Http;
using Microphone.WebApi;

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

    
    public class DefaultController : AutoRegisterApiController
    {
        [Route("/")]
        public string Get()
        {
            return "Service2";
        }
    }
}
