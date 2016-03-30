using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microphone.Core;
using Microphone.Core.ClusterProviders;
using Newtonsoft.Json.Linq;

namespace Microphone.Etcd
{
    public class EtcdProvider : IClusterProvider
    {
        private readonly int _ectdHeartbeart;

        private readonly int _ectdTtl;
        private readonly string _etcdHost;

        private readonly int _etcdPort;

        private string _serviceId;

        private string _serviceName;

        public EtcdProvider(int ttl, int heartBeat) : this("127.0.0.1", 2379, ttl, heartBeat)
        {
        }

        public EtcdProvider(string host, int port, int ttl, int heartBeat)
        {
            _etcdHost = host;
            _etcdPort = port;
            _ectdTtl = ttl;
            _ectdHeartbeart = heartBeat;
        }

        public async Task<ServiceInformation[]> FindServiceInstancesAsync(string serviceName)
        {
            var url = $"http://{_etcdHost}:{_etcdPort}/v2/keys/services/{serviceName}";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url).ConfigureAwait(false);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Could not find services");
                }
                Logger.Information("{ServiceName} lookup {OtherServiceName}", _serviceName, serviceName);
                var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var res = JObject.Parse(body);
                var nodes = res["node"]["nodes"];
                return (from node in nodes
                        let uriStr = node["value"].Value<string>()
                        where !string.IsNullOrEmpty(uriStr)
                        let uri = new Uri(uriStr)
                        select new ServiceInformation(uri.AbsoluteUri, uri.Port))
                        .ToArray();
            }
        }

        public async Task RegisterServiceAsync(string serviceName, string serviceId, string version, Uri uri)
        {
            _serviceName = serviceName;
            _serviceId = serviceId;

            Func<Task> registerService = async () =>
            {
                var url = $"http://{_etcdHost}:{_etcdPort}/v2/keys/services/{serviceName}/{serviceId}";
                using (var client = new HttpClient())
                {
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("value", uri.ToString()),
                        new KeyValuePair<string, string>("ttl", _ectdTtl.ToString())
                    });
                    await client.PutAsync(url, content).ConfigureAwait(false);
                }
            };

            await registerService().ConfigureAwait(false);

            StartHeartbeat(registerService);
        }

        public Task BootstrapClientAsync()
        {
            return Task.FromResult(0);
        }

        private void StartHeartbeat(Func<Task> registerService)
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await registerService().ConfigureAwait(false);
                    await Task.Delay(TimeSpan.FromSeconds(_ectdHeartbeart)).ConfigureAwait(false);
                    Logger.Information("OK");
                }
            });
        }
    }
}