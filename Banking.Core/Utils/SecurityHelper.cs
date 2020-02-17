using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Banking.Core.Models;

namespace Banking.Core.Utils
{
         public static class ExtensionMethods
        {
            public static IEnumerable<UserLogin> WithoutPasswords(this IEnumerable<UserLogin> users)
            {
                if (users == null) return null;

                return users.Select(x => x.WithoutPassword());
            }

            public static UserLogin WithoutPassword(this UserLogin user)
            {
                if (user == null) return null;

                user.Password = null;
                return user;
            }
        }
    
}
