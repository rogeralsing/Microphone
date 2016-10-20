using System.IO;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microphone.AspNet;
using Microphone.Consul;
using System.Net.Http;

namespace AspNetService
{
    public class Startup
    {
        private static readonly Lazy<string> Host = new Lazy<string>(() => RancherMetadata.GetHost());
        private static readonly Lazy<string> Port = new Lazy<string>(() => RancherMetadata.GetPort());
        public Startup(IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddMicrophone<ConsulProvider>();

            services.Configure<ConsulOptions>(o =>
            {
                o.Host = Startup.Host.Value;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory
            .AddConsole(Configuration.GetSection("Logging"))
            .AddDebug();

            var port = Startup.Port.Value;
            var host = Startup.Host.Value;
            Console.WriteLine($"Running on rancher host IP {host}");

            app
            .UseMvc()
            .UseMicrophone("AspNetService", "1.0", new Uri($"http://{host}:{port}"));
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            System.Threading.Thread.Sleep(10000); //Rancher metadata is not ready directly at startup :-(
            new WebHostBuilder()
                .UseKestrel()
                .UseUrls(new[] { "http://0.0.0.0:5000" })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }

    public static class RancherMetadata
    {


        public static string GetHost()
        {
            var host = HttpGet("http://rancher-metadata/2015-12-19/self/host/agent_ip");
            return host;
        }

        public static string GetPort() {
            var port = HttpGet("http://rancher-metadata/2015-12-19/self/service/ports/0").Split(':')[0];
            return port;
        }

        private static string HttpGet(string uri)
        {
            var http = new HttpClient();
            http.BaseAddress = new Uri(uri);
            var res = http.GetStringAsync("").Result;
            return res;
        }
    }
}
