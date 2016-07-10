namespace Microphone
{
    public class ServiceInformation
    {
        public ServiceInformation(string serviceAddress, int servicePort)
        {
            Address = serviceAddress;
            Port = servicePort;
        }

        public string Address { get; }
        public int Port { get; }
    }
}