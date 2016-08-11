using System.IO;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microphone.AspNet;
using System.Threading.Tasks;
using Microphone.Consul;
using Microphone;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;

namespace AspNetService
{
    public class Startup
    {
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
            services
                .AddMicrophone<ConsulProvider>()
                .AddHealthCheck<MyHealthChecker>();

                services.Configure<ConsulOptions>(o => {
                    o.Host = Configuration["ConsulHost"];
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory
            .AddConsole(Configuration.GetSection("Logging"))
            .AddDebug();

            var localIp = Labb.GetLocalIPAddress();

            if (Configuration["rancher"]=="true")
            {
                var rancherUri = new Uri("http://rancher-metadata/2015-12-19/self/container/primary_ip");
                var http = new HttpClient();
                http.BaseAddress = rancherUri;
                localIp = http.GetStringAsync("").Result;
                Console.WriteLine($"Running on rancher, {localIp}");
            }
            else
            {
                Console.WriteLine($"Running locally, {localIp}");
            }

            app
            .UseMvc()
            .UseMicrophone("AspNetService", "1.0",new Uri($"http://{localIp}:5000"));
        }

        public static void Main(string[] args)
        {
            new WebHostBuilder()
                .UseKestrel()
                .UseUrls(new[] { "http://0.0.0.0:5000" })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }

    public static class Labb
    {
        public static string GetLocalIPAddress() => GetLocalIPAddress(Dns.GetHostName());

        public static string GetLocalIPAddress(Uri uri) => GetLocalIPAddress(uri.Host);

        public static string GetLocalIPAddress(string hostName)
        {
            var host = Dns.GetHostEntryAsync(hostName).Result;
            var l = new List<string>();
            foreach (var ip in host.AddressList.OrderByDescending(ip => ip.ToString()))
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    l.Add(ip.ToString());
                }
            }

            return l.First();
        }
    }

    //adding this kind of extra healthcheck will allow you to
    //do additional service healthcheck, e.g. ping database
    public class MyHealthChecker : IHealthCheck
    {
        private ILogger _logger;
        public MyHealthChecker(ILoggerFactory loggerFactory)
        {
            //use the default aspnet core DI support
            _logger = loggerFactory.CreateLogger("MyHealthCheck");
        }
        public async Task CheckHealth()
        {
            await Task.Yield(); //just to show we can do async
            _logger.LogInformation("Additional HealthCheck");
        }
    }
}
