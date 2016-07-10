using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microphone.AspNet;
using System.Threading.Tasks;
using Microphone.Consul;
using Microphone;

namespace AspNetService
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services
                .AddMicrophone<ConsulProvider>()
                .AddHealthCheck<MyHealthChecker>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory
            .AddConsole(Configuration.GetSection("Logging"))
            .AddDebug();

            app
            .UseMvc()
            .UseMicrophone("AspNetService", "1.0");
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
