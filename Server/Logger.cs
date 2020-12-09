using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Server
{
    public static class Logger
    {
        private static ILogger _logger;

        static Logger()
        { 
            _logger = Initialize();
        }

        public static ILogger Get()
        {
            return _logger ??= Initialize();
        }

        private static Serilog.Core.Logger Initialize()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level:w3}] {Message}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Code
                )
                .CreateLogger();
        }

        public static void Info(string msg)      => _logger.Information(msg);

        public static void Warn(string msg)      => _logger.Warning(msg);

        public static void Error(string msg)     => _logger.Error(msg);

        public static void Exception(string msg) => _logger.Error($"EXCEPTION: {msg}");

        public static void Debug(string msg)     => _logger.Information($"DEBUG: {msg}");
    }
}
