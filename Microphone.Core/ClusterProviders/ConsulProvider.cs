using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var x = new ConsulRestClient();
            var res = x.FindServiceAsync(name).Result;
            Logger.Information("{ServiceName} lookup {OtherServiceName}", _serviceName, name);
            return res;
        }

        public void RegisterService(string serviceName, string serviceId, string version, Uri uri)
        {
            _serviceName = serviceName;
            _serviceId = serviceId;
            _version = version;
            _uri = uri;
            var x = new ConsulRestClient();
            x.RegisterServiceAsync(serviceName, serviceId, uri).Wait();
            StartReaper();
        }

        public string GetConfig()
        {

            //var client = new Client();
            //var key = "ServiceConfig:" + _serviceName;
            //var response = client.KV.Get(key);
            //var res = Encoding.UTF8.GetString(response.Response.Value);
            //return res;
            return "";
        }

        private void StartReaper()
        {
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(1000).ConfigureAwait(false);
                Logger.Information("Reaper: started..");
                var c = new ConsulRestClient();
                HashSet<string> lookup = new HashSet<string>();
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
