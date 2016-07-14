using System;
using Microsoft.Extensions.Logging;
using Nancy;

namespace Microphone.Nancy
{
    public sealed class StatusModule : NancyModule
    {
        public StatusModule(ILoggerFactory loggerFactory,IHealthCheck healthCheck = null)
        {
            var logger = loggerFactory.CreateLogger("Microphone.Nancy");
            Get("/status", async req =>
            {
                if (healthCheck != null)
                {
                    await healthCheck.CheckHealth();
                }
                logger.LogInformation("OK");
                return Environment.MachineName;
            });
        }
    }
}
