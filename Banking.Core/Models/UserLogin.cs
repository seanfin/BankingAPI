using System;
using System.Collections.Generic;
using System.Text;

using Banking.Core.Enums;

namespace Banking.Core.Models
{

        public class UserLogin
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
            public string Token { get; set; }
        }
    
}
