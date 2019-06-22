
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using Serilog.Debugging;
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.File;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.IO;
using ILogger = Serilog.ILogger;

namespace RecipeSavants.Microservices.Logging
{
    public class LogException
    {
        public string Application { get; set; }
        public Exception e { get; set; }

        public LogException()
        {
            e = new Exception();
        }
    }

    public class LogMessage
    {
        public string Application { get; set; }
        public string Message { get; set; }
    }

    public class LoggingClient : ILoggingClinet 
    {
        private ILoggerFactory _Factory = null;
        private ILogger log = null;
        private IConfiguration _config;
        private string Name { get; set; }

        public LoggingClient(IConfiguration config, string ApplicationName)
        {
            Name = ApplicationName;
            _config = config;
            SelfLog.Enable(Console.Error);
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                .WriteTo.Console(theme: SystemConsoleTheme.Literate)
                                //not persistant
                                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(_config.GetConnectionString("elasticsearch"))) // for the docker-compose implementation
                                {
                                     
                                    AutoRegisterTemplate = true,
                                    RegisterTemplateFailure = RegisterTemplateRecovery.IndexAnyway,
                                    FailureCallback = e => Console.WriteLine("Unable to submit event " + e.MessageTemplate),
                                    EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                                                       EmitEventFailureHandling.WriteToFailureSink |
                                                       EmitEventFailureHandling.RaiseCallback,
                                    FailureSink = new FileSink("./fail-{Date}.txt", new JsonFormatter(), null, null)
                                })
                    .CreateLogger();
        }

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
            
            Log.Debug(JsonConvert.SerializeObject(new LogMessage() { Application = Name, Message = message }));
            Log.CloseAndFlush();
        }

        public void LogWarning(string warning)
        {
            Log.Warning(JsonConvert.SerializeObject(new LogMessage() { Application = Name, Message = warning }));
            Log.CloseAndFlush();
        }

        public void LogException(Exception e, string message)
        {
            Log.Error(JsonConvert.SerializeObject(new LogException() { Application = Name, e = e }));
            Log.CloseAndFlush();
        }
    }
}
