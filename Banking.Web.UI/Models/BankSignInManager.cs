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
using System.Net.Http;

using Banking.Core.Utils;
using Banking.Core.Models;


using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Headers;






namespace Banking.Web.UI.Models
{
    public class BankSignInManager<TUser> : SignInManager<TUser> where TUser : class
    {
        private readonly UserManager<TUser> _userManager;
       
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly ExternalAppSettings externalAppSettings;


        public BankSignInManager(UserManager<TUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<TUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<TUser>> logger, IAuthenticationSchemeProvider authenticationScehmeProvider, IUserConfirmation<TUser> confirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger,  authenticationScehmeProvider,  confirmation )
        {

            externalAppSettings = AppSettingHelper.GetExternalApplicationConfiguration();

            //if (userManager == null)
            //    throw new ArgumentNullException(nameof(userManager));

            //if (dbContext == null)
            //    throw new ArgumentNullException(nameof(dbContext));

            //if (contextAccessor == null)
            //    throw new ArgumentNullException(nameof(contextAccessor));

            _userManager = userManager;
            _contextAccessor = contextAccessor;
           
        }

        public override Task<SignInResult> CheckPasswordSignInAsync(TUser user, string password, bool lockoutOnFailure)
        {
            return base.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
        }

        public override Task SignInAsync(TUser user, bool isPersistent, string authenticationMethod = null)
        {
            return base.SignInAsync(user, isPersistent, authenticationMethod);
        }

        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {

            

            var resultofclient = SignInResult.NotAllowed;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.externalAppSettings.WebApiURLUser);

                AuthenticateModel model = new AuthenticateModel();
                model = new AuthenticateModel();
                model.Username = userName;
                model.Password = password;

                //HTTP GET
                var responseTask = client.PostAsJsonAsync("authenticate", model);
                responseTask.Wait();

                var authResult = responseTask.Result;
                if (authResult.IsSuccessStatusCode)
                {
                    resultofclient = SignInResult.Success;


                    var userLogin = await authResult.Content.ReadAsAsync<AuthenticateModel>();

                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Email, userLogin.Username));
                    claims.Add(new Claim(ClaimTypes.Role, userLogin.Role));
                    claims.Add(new Claim(ClaimTypes.Hash, userLogin.Token));



                    //CookieAuthenticationDefaults.AuthenticationScheme
                    var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    //identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));

                    //foreach (var role in user.Roles)
                    //{
                    userIdentity.AddClaim(new Claim(ClaimTypes.Role, userLogin.Role));

                    HttpClient bankClient = new HttpClient();
                    bankClient.BaseAddress = new Uri(this.externalAppSettings.WebApiURLBankTransaction);



                    //client.DefaultRequestHeaders.Add("IDENTITY_KEY", securityToken);

                    AuthenticationHeaderValue authHeaders = new AuthenticationHeaderValue("Bearer", userLogin.Token);
                    bankClient.DefaultRequestHeaders.Authorization = authHeaders;


                    var bankTransResults = await bankClient.GetAsync("Hello");






                    var authProperties = new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.Now.AddMinutes(3),
                        //IsPersistent = true,

                    };


                    var principal = new ClaimsPrincipal(userIdentity);
                    await this._contextAccessor.HttpContext.SignInAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                        principal: principal, properties: authProperties);



                }
                else
                {
                    resultofclient = SignInResult.Failed; 

                }



            }



            //var finalResult = Task.FromResult(resultofclient);

            //var result =  base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);

            return resultofclient; 
        }

     

        public override Task<bool> CanSignInAsync(TUser user)
        {
            return base.CanSignInAsync(user);
        }


        public override async Task<SignInResult> PasswordSignInAsync(TUser user, string password, bool isPersistent, bool lockoutOnFailure)
        {

           



                var result = await base.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);

            //var appUser = user as IdentityUser;

            //if (appUser != null) // We can only log an audit record if we can access the user object and it's ID
            //{
            //    var ip = _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            //    UserAudit auditRecord = null;

            //    switch (result.ToString())
            //    {
            //        case "Succeeded":
            //            auditRecord = UserAudit.CreateAuditEvent(appUser.Id, UserAuditEventType.Login, ip);
            //            break;

            //        case "Failed":
            //            auditRecord = UserAudit.CreateAuditEvent(appUser.Id, UserAuditEventType.FailedLogin, ip);
            //            break;
            //    }

            //    if (auditRecord != null)
            //    {
            //        _db.UserAuditEvents.Add(auditRecord);
            //        await _db.SaveChangesAsync();
            //    }
            //}

            return result;
        }
     

        
        /// <summary>
        /// Sign Out  of the system. 
        /// </summary>
        /// <returns>Nothing.</returns>
        public override async Task SignOutAsync()
        {
            //Need to sign out with my cookies because I am using cookies. 
            await this._contextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            //Perform what the base does. 
            await base.SignOutAsync();

        }

        public override Task<ClaimsPrincipal> CreateUserPrincipalAsync(TUser user)
        {
            return base.CreateUserPrincipalAsync(user);
        }
    }
}
