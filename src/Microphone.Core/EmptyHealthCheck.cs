using System.Threading.Tasks;

namespace Microphone.Core
{
    public class EmptyHealthCheck : IHealthCheck
    {
        public Task CheckHealth()
        {
            return Task.FromResult(0);
        }
    }
}
