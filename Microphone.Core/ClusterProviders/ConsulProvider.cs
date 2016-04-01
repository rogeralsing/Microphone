using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microphone.Core.ClusterProviders
{
    public class ConsulProvider : IClusterProvider
    {
        private readonly string _consulHost;
        private readonly int _consulPort;
        private readonly bool _useEbayFabio;
        private string _serviceId;
        private string _serviceName;
        private Uri _uri;
        private string _version;

        public ConsulProvider(string consulHost = "localhost", int consulPort = 8500, bool useEbayFabio = false)
        {
            _consulHost = consulHost;
            _consulPort = consulPort;
            _useEbayFabio = useEbayFabio;
        }

        public async Task<ServiceInformation[]> FindServiceInstancesAsync(string name)
        {
            if (_useEbayFabio)
            {
                return new[] {new ServiceInformation($"http://{_consulHost}", 9999)};
            }


            using (var client = new HttpClient())
            {
                var response =
                    await
                        client.GetAsync($"http://{_consulHost}:{_consulPort}/v1/health/service/" + name)
                            .ConfigureAwait(false);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Could not find services");
                }

                var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var res1 = JArray.Parse(body);
                var res =
                    res1.Select(
                        entry =>
                            new ServiceInformation(
                                entry["Service"]["Address"].Value<string>(),
                                entry["Service"]["Port"].Value<int>())
                        ).ToArray();

                return res;
            }
        }

        public async Task RegisterServiceAsync(string serviceName, string serviceId, string version, Uri uri)
        {
            _serviceName = serviceName;
            _serviceId = serviceId;
            _version = version;
            _uri = uri;
            var payload = new
            {
                ID = serviceId,
                Name = serviceName,
                Tags = new[] {$"urlprefix-/{serviceName}"},
                Address = Dns.GetHostName(),
                // ReSharper disable once RedundantAnonymousTypePropertyName
                Port = uri.Port,
                Check = new
                {
                    HTTP = uri + "status",
                    Interval = "1s"
                }
            };

            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json);

                var res =
                    await
                        client.PostAsync($"http://{_consulHost}:{_consulPort}/v1/agent/service/register", content)
                            .ConfigureAwait(false);
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Could not register service");
                }
            }
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

                var lookup = new HashSet<string>();
                while (true)
                {
                    try
                    {
                        IEnumerable<string> res;
                        using (var client1 = new HttpClient())
                        {
                            var response1 =
                                await
                                    client1.GetAsync($"http://{_consulHost}:{_consulPort}/v1/health/state/critical")
                                        .ConfigureAwait(false);
                            if (response1.StatusCode != HttpStatusCode.OK)
                            {
                                throw new Exception("Could not get service health");
                            }
                            var body = await response1.Content.ReadAsStringAsync().ConfigureAwait(false);
                            var res1 = JArray.Parse(body);
                            res = res1.Select(service => service["ServiceID"].Value<string>()).ToArray();
                        }
                        foreach (var criticalServiceId in res)
                        {
                            if (lookup.Contains(criticalServiceId))
                            {
                                using (var client = new HttpClient())
                                {
                                    var response =
                                        await
                                            client.GetAsync(
                                                $"http://{_consulHost}:{_consulPort}/v1/agent/service/deregister/" +
                                                criticalServiceId).ConfigureAwait(false);
                                    if (response.StatusCode != HttpStatusCode.OK)
                                    {
                                        throw new Exception("Could not de register service");
                                    }
                                }
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