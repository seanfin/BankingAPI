﻿using System;
using System.Collections.Generic;
using System.Text;

using Banking.Core.Models;

namespace Banking.Core.Interfaces
{
    public interface IUserService
    {
        UserLogin Authenticate(string username, string password);
        IEnumerable<UserLogin> GetAll(bool withoutPasswords = true);
        UserLogin GetById(Guid id);
    }
}