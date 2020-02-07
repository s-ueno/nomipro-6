using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace OrchestrationFunctons.Core
{
    public static class AppUtil
    {
        static AppUtil()
        {
            // https://stackoverflow.com/questions/39573571/net-core-console-application-how-to-configure-appsettings-per-environment
            // http://michaco.net/blog/EnvironmentVariablesAndConfigurationInASPNETCoreApps
            // 柔軟性から、GetEnvironmentVariableよりもConfigurationBuilderが進められているので、そっちにする
            var builder = new ConfigurationBuilder()
                            .AddJsonFile("local.settings.json", true)
                            .AddEnvironmentVariables();

            _configuration = builder.Build();
        }
        private static readonly IConfigurationRoot _configuration;


        public static T GetAppConfigValue<T>(string key, T defaultValue = default(T))
        {
            try
            {
                var val = _configuration[key];
                if (string.IsNullOrWhiteSpace(val))
                    return defaultValue;

                defaultValue = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(val);
            }
            catch
            {
            }
            return defaultValue;
        }

    }

}
