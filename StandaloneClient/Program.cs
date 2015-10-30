using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microphone.Core;
using Microphone.Core.ClusterProviders;

namespace StandaloneClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Cluster.BootstrapClient(new ConsulProvider(useEbayFabio: false));

            while (true)
            {
                Console.ReadLine();
                var res = Cluster.FindServiceInstanceAsync("orders").Result;
                Console.WriteLine("Found Address:{0} Port:{1}",res.Address,res.Port);
            }
        }
    }
}
