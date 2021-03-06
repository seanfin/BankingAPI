﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Banking.Core.Models
{
    
        public class AuthenticateModel
        {

            public Guid Id { get; set; }

            [Required]
            public string Username { get; set; }

            [Required]
            public string Password { get; set; }

            public string Token { get; set; }

            public string Role { get; set; }


       

    }

   

}
