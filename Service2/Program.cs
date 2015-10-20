using System;
using System.Web.Http;
using Microphone.Core;
using Microphone.Core.ClusterProviders;
using Microphone.WebApi;

namespace Service2
{
    class Program
    {
        static void Main(string[] args)
        {
            Cluster.Bootstrap<WebApiProvider, ConsulProvider>("Service2", "v1");
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
