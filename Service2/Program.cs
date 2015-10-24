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
            Cluster.Bootstrap(new WebApiProvider(), new ConsulProvider(useEbayFabio: false), "orders", "v1");
            Console.ReadLine();
        }
    }

    public class OrdersController : ApiController
    {

        public Order Get()
        {
            return new Order
            {
                CustomerId = 123,
                OrderDate = DateTime.Now,
            };
        }
    }

    public class Order
    {
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }

    }
}
