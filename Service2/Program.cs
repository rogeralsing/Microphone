using System;
using System.Web.Http;
using Microphone.WebApi;

namespace Service2
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrap.Start("Service2","v1");
            Console.ReadLine();
        }
    }
    
    public class DefaultController : ApiController
    {
        public string Get()
        {
            return "Service2";
        }
    }
}
