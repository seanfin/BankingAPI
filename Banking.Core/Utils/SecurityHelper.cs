using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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

    }

}
