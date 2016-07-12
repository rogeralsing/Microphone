namespace Microphone
{
    public class ServiceInformation
    {
        public ServiceInformation(string serviceAddress, int servicePort)
        {
            Host = serviceAddress;
            Port = servicePort;
        }

        public string Host { get; }
        public int Port { get; }
    }
}