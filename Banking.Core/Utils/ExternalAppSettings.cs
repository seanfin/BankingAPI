﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Banking.Core.Utils
{
    public class ExternalAppSettings
    {
        
        /// <summary>
        /// This is the URL for the web api for the BankTrans action Service
        /// </summary>
        public string WebApiURLBankTransaction { get; set; }

        /// <summary>
        /// This is the URL for the web api for the User Service
        /// </summary>
        public string WebApiURLUser { get; set; }

        /// <summary>
        /// This is the URL for the web api for the User Service
        /// </summary>
        public string WebApiURLProfile { get; set; }

        /// <summary>
        /// This is how long we will persist the user. 
        /// </summary>
        public int MinutesToPersistUser { get; set; }

    }
}
