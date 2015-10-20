using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consul;

namespace Microphone.Core.ClusterProviders
{
    public class ConsulProvider : IClusterProvider
    {
        private string _serviceName;
        private string _serviceId;
        private string _version;
        private Uri _uri;

        public ServiceInformation[] FindService(string name)
        {
            Logger.Information("{ServiceName} lookup {OtherServiceName}", _serviceName, name);
            var client = new Client();
            var others = client.Health.Service(name, null, true);

            return
                others.Response.Select(other => new ServiceInformation(other.Service.Address, other.Service.Port))
                    .ToArray();
        }

        public void RegisterService(string serviceName, string serviceId, string version, Uri uri)
        {
            _serviceName = serviceName;
            _serviceId = serviceId;
            _version = version;
            _uri = uri;
            var client = new Client();
            client.Agent.ServiceRegister(new AgentServiceRegistration
            {
                Address = uri.Host,
                ID = serviceId,
                Name = serviceName,
                Port = uri.Port,
                Tags = new[] { version },
                Check = new AgentServiceCheck
                {
                    HTTP = uri + "status",
                    Interval = TimeSpan.FromSeconds(1),
                    TTL = TimeSpan.Zero,
                    Timeout = TimeSpan.Zero
                }
            });
            StartReaper();
        }

        public string GetConfig()
        {
            var client = new Client();
            var key = "ServiceConfig:" + _serviceName;
            var response = client.KV.Get(key);
            var res = Encoding.UTF8.GetString(response.Response.Value);
            return res;
        }

        private void StartReaper()
        {
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(1000).ConfigureAwait(false);
                Logger.Information("Reaper: started..");
                var client = new Client();
                var lookup = new HashSet<string>();
                while (true)
                {
                    try
                    {
                        var checks = client.Agent.Checks();
                        foreach (var check in checks.Response)
                        {
                            if (Equals(check.Value.Status, CheckStatus.Critical))
                            {
                                //dont delete new services
                                if (lookup.Contains(check.Value.ServiceID))
                                {
                                    client.Agent.ServiceDeregister(check.Value.ServiceID);
                                    Logger.Information("Reaper: Removing {ServiceId}", check.Value.ServiceID);
                                }
                                else
                                {
                                    Logger.Information("Reaper: Marking {ServiceId}", check.Value.ServiceID);
                                    lookup.Add(check.Value.ServiceID);
                                }

                            }
                            else
                            {
                                //if service is ok, remove it from reaper set
                                lookup.Remove(check.Value.ServiceID);
                            }
                        }
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
