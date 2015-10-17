using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consul;

namespace ServiceCleanup
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client();
            var lookup = new HashSet<string>();
            while (true)
            {
                var checks = client.Agent.Checks();
                foreach (var check in checks.Response)
                {
                    if (Equals(check.Value.Status, CheckStatus.Critical))
                    {
                        //dont delete new services
                        if (lookup.Contains(check.Value.ServiceID))
                        {
                            client.Agent.ServiceDeregister(check.Value.ServiceID);
                            Console.WriteLine("Unregistering service {0}", check.Value.ServiceID);
                        }
                        else
                        {
                            lookup.Add(check.Value.ServiceID);
                        }

                    }
                }

                Task.Delay(1000).Wait();
            }

            //var client = new Client();
            //var allServices = client.Catalog.Services();
            //foreach (var service in allServices.Response.Where(s => s.Key != "consul"))
            //{

            //    var health = client.Health.Service(service.Key);

            //    foreach (var serviceInstance in health.Response)
            //    {

            //        client.Agent.ServiceDeregister(serviceInstance.Service.ID);
            //    }                
            //}
        }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Consul;

//namespace ServiceCleanup
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            var client = new Client();
//            while (true)
//            {
//                var allServices = client.Catalog.Services();
//                foreach (var service in allServices.Response.Where(s => s.Key != "consul"))
//                {
//                    var checks = client.Agent.Checks();
//                    foreach (var check in checks.Response)
//                    {
//                        if (Equals(check.Value.Status, CheckStatus.Critical))
//                        {
//                            client.Agent.ServiceDeregister(service.Key);
//                            client.Agent.CheckDeregister(check.Key);
//                            Console.WriteLine("Unregistering service {0}", service.Key);
//                        }
//                    }
//                }
//                Task.Delay(1000).Wait();
//            }
//        }
//    }
//}
