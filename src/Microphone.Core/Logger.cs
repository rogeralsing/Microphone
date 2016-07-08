using System;
using Serilog;

namespace Microphone.Core
{
    public static class Logger
    {
        private static readonly ILogger _logger;

        static Logger()
        {
            _logger = new LoggerConfiguration().CreateLogger();
        }

        public static void Debug(string template, params object[] args)
        {
            _logger.Debug(template, args);
        }

        public static void Information(string template, params object[] args)
        {
            _logger.Information(template, args);
        }

        public static void Error(string template, params object[] args)
        {
            _logger.Error(template, args);
        }

        public static void Error(Exception cause, string template, params object[] args)
        {
            _logger.Error(cause, template, args);
        }

        public static void Warning(string template, params object[] args)
        {
            _logger.Warning(template, args);
        }
    }
}