using Microsoft.Extensions.Logging;
using System;

namespace RecipeSavants.Microservices.Logging
{
    public class LoggingClient : ILoggingClinet 
    {
        private ILoggerFactory _Factory = null;
        private ILogger log = null;

        public ILoggerFactory LoggerFactory
        {
            get
            {
                return _Factory;
            }
            set { _Factory = value; }
        }

        public void LogDebug(string message)
        {
            log = CreateLogger();
            log.LogDebug(message);
            log = null;
        }

        public void LogWarning(string warning)
        {
            log.LogWarning(warning);
        }
        public void LogException(Exception e, string message)
        {
            log.LogError(e, message);
        }

        public ILogger CreateLogger() => log = LoggerFactory.CreateLogger("ApplicationName");
    }
}
