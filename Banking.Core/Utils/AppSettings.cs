using System;
using System.Collections.Generic;
using System.Text;

namespace Banking.Core.Utils
{
    public class AppSettings
    {
        /// <summary>
        /// This is the secret
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// This is the time limit for the cache.
        /// </summary>
        public int CacheExpirationInMinutes { get; set; }
    }
}
