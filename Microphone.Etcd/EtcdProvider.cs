using System;
using System.Configuration;
using System.Collections.Generic;
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
        private readonly string etcdHost;

        private readonly int etcdPort;

        private readonly int ectdTtl;

        private readonly int ectdHeartbeart;

        private string _serviceId;

        private string _serviceName;

        public EtcdProvider()
        {
            etcdHost = ConfigurationManager.AppSettings["etcd:Host"].ToString();
            etcdHost = etcdHost == string.Empty ? @"127.0.0.1" : etcdHost;

            Int32.TryParse(ConfigurationManager.AppSettings["etcd:Port"], out etcdPort);
            etcdPort = etcdPort == 0 ? 2379 : etcdPort;

            Int32.TryParse(ConfigurationManager.AppSettings["etcd:Ttl"], out ectdTtl);
            ectdTtl = ectdTtl == 0 ? 5 : ectdTtl;

            Int32.TryParse(ConfigurationManager.AppSettings["etcd:Heartbeat"], out ectdHeartbeart);
            ectdHeartbeart = ectdHeartbeart == 0 ? 1 : ectdHeartbeart;
        }

        public EtcdProvider(int ttl, int heartBeat)
        {
            etcdHost = "127.0.0.1";
            etcdPort = 2379;
            ectdTtl = ttl;
            ectdHeartbeart = heartBeat;
        }

        public EtcdProvider(string host, int port, int ttl, int heartBeat)
        {
            etcdHost = host;
            etcdPort = port;
            ectdTtl = ttl;
            ectdHeartbeart = heartBeat;
        }

        public async Task<ServiceInformation[]> FindServiceInstancesAsync(string serviceName)
        {
            var url = $"http://{etcdHost}:{etcdPort}/v2/keys/services/{serviceName}";
            var client = new HttpClient();
            var response = await client.GetAsync(url).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Could not find services");
            }
            Logger.Information("{ServiceName} lookup {OtherServiceName}", _serviceName, serviceName);
            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var res = JObject.Parse(body);
            var nodes = res["node"]["nodes"];
            var list = new List<ServiceInformation>();
            foreach (var node in nodes)
            {
                var uriStr = node["value"].Value<string>();
                if (string.IsNullOrEmpty(uriStr))
                    continue;

                var uri = new Uri(uriStr);
                list.Add(new ServiceInformation(uri.AbsoluteUri, uri.Port));
            }
            return list.ToArray();
        }

        public async Task RegisterServiceAsync(string serviceName, string serviceId, string version, Uri uri)
        {
            _serviceName = serviceName;
            _serviceId = serviceId;

            Func<Task> registerService = async () =>
            {
                var url = $"http://{etcdHost}:{etcdPort}/v2/keys/services/{serviceName}/{serviceId}";
                var client = new HttpClient();
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("value", uri.ToString()),
                    new KeyValuePair<string, string>("ttl", ectdTtl.ToString())
                });
                var response = await client.PutAsync(url, content).ConfigureAwait(false);
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
                    await Task.Delay(TimeSpan.FromSeconds(ectdHeartbeart)).ConfigureAwait(false);
                    Logger.Information("OK");
                }
            });
        }
    }
}