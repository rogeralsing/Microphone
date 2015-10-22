using System;
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
        private string _serviceId;

        private string _serviceName;

        public async Task<ServiceInformation[]> FindServiceAsync(string serviceName)
        {
            var url = $"http://127.0.0.1:2379/v2/keys/services/{serviceName}";
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
                var url = $"http://127.0.0.1:2379/v2/keys/services/{serviceName}/{serviceId}";
                var client = new HttpClient();
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("value", uri.ToString()),
                    new KeyValuePair<string, string>("ttl", "5")
                });
                var response = await client.PutAsync(url, content).ConfigureAwait(false);
            };

            await registerService().ConfigureAwait(false);

            StartHeartbeat(registerService);
        }

        private static void StartHeartbeat(Func<Task> registerService)
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await registerService().ConfigureAwait(false);
                    await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                    Logger.Information("OK");
                }
            });
        }
    }
}