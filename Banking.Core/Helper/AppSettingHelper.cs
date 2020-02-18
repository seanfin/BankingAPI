using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Banking.Core.Utils
{
    public class AppSettingHelper
    {       

        public static IConfigurationRoot GetIConfigurationRoot()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            return builder;



        }


        public static AppSettings GetApplicationConfiguration()
        {          

            var configuration = new AppSettings();
            var iConfig = GetIConfigurationRoot();
            var section = iConfig.GetSection("AppSettings");
                       
            ConfigurationBinder.Bind(section, configuration);
                       
            return configuration;
        }

        public static ExternalAppSettings GetExternalApplicationConfiguration()
        {

            var configuration = new AppSettings();
            var iConfig = GetIConfigurationRoot();
            var section = iConfig.GetSection("ExternalAppSettings");

            ConfigurationBinder.Bind(section, configuration);

            return configuration;
        }





    }
}
