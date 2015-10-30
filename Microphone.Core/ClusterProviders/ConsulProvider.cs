using Microphone.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microphone.Core.ClusterProviders
{
    public class ConsulProvider : IClusterProvider
    {
        private string _serviceId;
        private string _serviceName;
        private Uri _uri;
        private string _version;
        private readonly int _consulPort = 0;
        private readonly bool _useEbayFabio;

        public ConsulProvider(int port = 0, bool useEbayFabio = false)
        {
            _consulPort = port;
            _useEbayFabio = useEbayFabio;
        }

        public async Task<ServiceInformation[]> FindServiceInstancesAsync(string name)
        {
            if (_useEbayFabio)
            {
                return new[] { new ServiceInformation("http://localhost", 9999) };
            }

            var x = new ConsulRestClient();
            var res = await x.FindServiceAsync(name).ConfigureAwait(false);

            return res;
        }

        public async Task RegisterServiceAsync(string serviceName, string serviceId, string version, Uri uri)
        {
            _serviceName = serviceName;
            _serviceId = serviceId;
            _version = version;
            _uri = uri;
            var x = new ConsulRestClient();
            await x.RegisterServiceAsync(serviceName, serviceId, uri).ConfigureAwait(false);
            StartReaper();
        }

        public Task BootstrapClientAsync()
        {
            StartReaper();
            return Task.FromResult(0);
        }

        private void StartReaper()
        {
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(1000).ConfigureAwait(false);
                Logger.Information("Reaper: started..");

                var c = _consulPort > 0 ? new ConsulRestClient(_consulPort) : new ConsulRestClient();

                var lookup = new HashSet<string>();
                while (true)
                {
                    try
                    {
                        var res = await c.GetCriticalServicesAsync().ConfigureAwait(false);
                        foreach (var criticalServiceId in res)
                        {
                            if (lookup.Contains(criticalServiceId))
                            {
                                await c.DeregisterServiceAsync(criticalServiceId).ConfigureAwait(false);
                                Logger.Information("Reaper: Removing {ServiceId}", criticalServiceId);
                            }
                            else
                            {
                                lookup.Add(criticalServiceId);
                                Logger.Information("Reaper: Marking {ServiceId}", criticalServiceId);
                            }
                        }
                        //remove entries that are no longer critical
                        lookup.RemoveWhere(i => !res.Contains(i));
                    }
                    catch (Exception x)
                    {
                        Logger.Error(x, "Crashed");
                    }

                    await Task.Delay(5000).ConfigureAwait(false);
                }
            });
        }
    }
}