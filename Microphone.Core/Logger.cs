using Serilog;
using Serilog.Events;

namespace Microphone.Core
{
    public class Logger
    {
        private static readonly ILogger _logger;

        static Logger()
        {
            _logger = new LoggerConfiguration().WriteTo.ColoredConsole(LogEventLevel.Debug).CreateLogger();
        }


        public void Debug(string template, params object[] args)
        {
            _logger.Debug(template,args);
        }

        public void Information(string template, params object[] args)
        {
            _logger.Information(template, args);
        }

        public void Error(string template, params object[] args)
        {
            _logger.Error(template, args);
        }

        public void Warning(string template, params object[] args)
        {
            _logger.Warning(template, args);
        }
    }
}