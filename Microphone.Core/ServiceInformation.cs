namespace Microphone.Core
{
    public class ServiceInformation
    {
        public ServiceInformation(string serviceAddress, int servicePort)
        {
            ServiceAddress = serviceAddress;
            ServicePort = servicePort;
        }

        public string ServiceAddress { get; }
        public int ServicePort { get; }
    }
}