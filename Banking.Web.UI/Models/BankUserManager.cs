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
using System.Security.Claims;


using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;

using Banking.Core.Models;
using Banking.Core.Utils;



namespace Banking.Web.UI.Models
{
    public class BankUserManager<TUser> :   UserManager<TUser> where TUser : class
    {
        private readonly ExternalAppSettings externalAppSettings;

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

            externalAppSettings = AppSettingHelper.GetExternalApplicationConfiguration();
        }

                       

        public override Task<TUser> FindByEmailAsync(string email)
        {

            return base.FindByNameAsync(email);

            
        }


        public override Task<TUser> FindByNameAsync(string userName)
        {

            ///This tries to look up the user via a database we don't have that so let's null it out for this.
            return Task.FromResult((TUser)null);

        }

    }
}
