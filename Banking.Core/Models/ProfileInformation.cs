using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

using Banking.Core.Enums;

namespace Banking.Core.Models
{

    public class ProfileInformation
      
   

    {
        public Guid Id { get; set; }
                
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }

        [Required]
        public IEnumerable<int> BankAccountNumbers { get; set; }

        

    }

    

}
