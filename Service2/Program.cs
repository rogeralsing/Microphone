using System;
using System.Web.Http;
using Microphone.Core;
using Microphone.Core.ClusterProviders;
using Microphone.WebApi;

namespace WebApiServiceExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Cluster.Bootstrap<WebApiProvider, ConsulProvider>("WebApiServiceExample", "v1");
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
