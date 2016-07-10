using System.Threading.Tasks;

namespace Microphone
{
    public interface IHealthCheck
    {
        Task CheckHealth();
    }
}