using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microphone.AspNet;
using Microphone.Core.ClusterProviders;
using System.Threading.Tasks;

namespace AspNetService
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
         //   services.AddTransient<ICheckHealth,MyHealthChecker>(); //use additional healthchecks
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseMvc();
            app.UseMvcWithDefaultRoute();
            var consulHost = Configuration["CONSULHOST"] ?? "localhost"; 
            var consulPort = int.Parse(Configuration["CONSULPORT"] ?? "8500");   
            var consulProvider = new ConsulProvider(consulHost,consulPort);       
            app.UseMicrophone(loggerFactory,consulProvider, "AspNetService", "1.0");
        }

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }

    //adding this kind of extra healthcheck will allow you to do additional service healthcheck, e.g. ping database
    public class MyHealthChecker : ICheckHealth
    {
        private ILogger _logger;
        public MyHealthChecker(ILoggerFactory loggerFactory){
            _logger = loggerFactory.CreateLogger("MyHealthCheck");
        }
        public async Task CheckHealth()
        {
            await Task.Yield(); //just to show we can do async
            _logger.LogInformation("Additional HealthCheck");
        }
    }
}