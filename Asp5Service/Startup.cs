using System;
using Microphone.AspNet;
using Microphone.Core.ClusterProviders;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetService
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseIISPlatformHandler();
            app.UseMvc();
            var consulHost = Configuration["CONSULHOST"] ?? "localhost";
            app.UseMicrophone(new ConsulProvider(consulHost: consulHost), "AspNetService", "1.0");
        }

        // Entry point for the application.
        public static void Main(string[] args)
        {
            //    var host = new WebHostBuilder()
            //        .CaptureStartupErrors(true)
            //        .UseDefaultHostingConfiguration(args)
            //        .UseServer("Microsoft.AspNetCore.Server.Kestrel")
            //        .UseUrls(new[] { "https://localhost:11111" })
            //.Build();

            WebApplication.Run<Startup>(args);
        }
    }
}