using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;



using Banking.Core.Models;

namespace Banking.Core.Utils
{
    public static class SecurityHelper
    {


        public static AuthenticateModel RemovePassword(AuthenticateModel authenticateModel)
        {
            if (authenticateModel == null)
                return null;


            authenticateModel.Password = null;
            return authenticateModel;

        }

        public static string GetMaskedAccountNumber(int accountNumber)
        {
           var maskedAcctNumber =  accountNumber.ToString().Substring(accountNumber.ToString().Length - 4);
             return  "..." + maskedAcctNumber;
        }

        public static T DeepClone<T>(this T obj)
        {
            var serialized = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        


        }

}
