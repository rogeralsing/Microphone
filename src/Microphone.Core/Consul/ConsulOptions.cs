namespace Microphone.Consul
{
    public class ConsulOptions
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 8500;
        public ConsulNameResolution NameResolution { get; set; } = ConsulNameResolution.HttpApi;
        public int Heartbeat { get; set; } = 1;
        public string HealthCheckPath { get; set; } = "/status";
    }
}