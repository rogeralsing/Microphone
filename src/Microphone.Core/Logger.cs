using Microsoft.Extensions.Logging;

namespace Microphone.Core
{
    public static class Logger
    {
        public static ILogger Log {get;private set;}

        static Logger()
        {
            Log = null;
        }

        public static void SetLogger(ILogger logger){
            Log = logger;
        }
    }
}