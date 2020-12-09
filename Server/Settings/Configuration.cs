using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Settings
{
    public static class Configuration
    {
        private static IConfigurationRoot config;

        static Configuration()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env}.json", true, true)
                .AddEnvironmentVariables();
            config = builder.Build();
        }

        public static AppSettings Get()
        {
            return config.Get<AppSettings>();
        }
    }
}
