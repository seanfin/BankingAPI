using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Extensions.Configuration;



using Banking.Core.Utils;

namespace Banking.Core.Test
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





    }
}
