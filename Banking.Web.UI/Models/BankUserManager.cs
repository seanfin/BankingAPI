using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using System.Net;


namespace Banking.Web.UI.Models
{
    public class BankUserManager<TUser> :   UserManager<TUser> where TUser : class
    {
        IHttpContextAccessor _httpContext; 

        public BankUserManager(IHttpContextAccessor httpContext, IUserStore<TUser> userStore, IOptions<IdentityOptions> optionsAccessor,
       IPasswordHasher<TUser> passwordHasher,
       IEnumerable<IUserValidator<TUser>> userValidators,
       IEnumerable<IPasswordValidator<TUser>> passwordValidators, ILookupNormalizer keyNormalizer,
       IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<TUser>> logger) :
       base(userStore, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors,
           services, logger)
        {
            this._httpContext = httpContext;


        }


       

        public override Task<TUser> FindByNameAsync(string userName)
        {

            //Let's look it up via Rest call 



            return Task.FromResult((TUser)null);






            //return base.FindByNameAsync(userName);
        }

    }
}
