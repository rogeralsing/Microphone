using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microphone;
using Microphone.Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ConsoleApplication1
{
    class Provider : IFrameworkProvider
    {
        public Uri GetUri()
        {
            return new Uri("http://localhost:5555");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var s = new ServiceCollection();
            s.AddLogging();
            s.AddOptions();
            s.AddSingleton<IFrameworkProvider, Provider>();
            s.AddSingleton<IClusterProvider, ConsulProvider>();

            var ioc = s.BuildServiceProvider();
            var frameworkProvider = ioc.GetService<IFrameworkProvider>();
            var clusterProvider = ioc.GetService<IClusterProvider>();

            var f = new Microsoft.Extensions.Logging.LoggerFactory();
            var l = f.CreateLogger("foo");
            Cluster.RegisterService(frameworkProvider, clusterProvider, "abc","1",l);
        }
    }
}
