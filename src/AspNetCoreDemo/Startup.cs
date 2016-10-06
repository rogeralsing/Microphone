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

            services.Configure<ConsulOptions>(o =>
            {
                var host = UriResolver.GetHost();
                o.Host = host;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory
            .AddConsole(Configuration.GetSection("Logging"))
            .AddDebug();

            string port = "5000";
            var host = UriResolver.GetHost();
            Console.WriteLine($"Running on rancher host IP {host}");
            var tags = new[] { "foo", "bar" };

            app
            .UseMvc()
            .UseMicrophone("AspNetService", "1.0", new Uri($"http://{host}:{port}"), tags);
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            System.Threading.Thread.Sleep(20000);
            new WebHostBuilder()
                .UseKestrel()
                .UseUrls(new[] { "http://0.0.0.0:5000" })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build()
                .Run();
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

    public static class UriResolver
    {


        public static string GetHost()
        {
            var host = HttpGet("http://rancher-metadata/2015-12-19/self/host/agent_ip");
            return host;
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
