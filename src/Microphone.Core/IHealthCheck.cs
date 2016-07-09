using System.Threading.Tasks;

namespace Microphone.Core
{
    public interface IHealthCheck
    {
        Task CheckHealth();
    }
}