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
                       
            _userManager = userManager;
            _contextAccessor = contextAccessor;
           
        }

            
        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var resultofclient = SignInResult.NotAllowed;

            using (var client = new HttpClient())
            {
                //Getting access to our web api. 
                client.BaseAddress = new Uri(this.externalAppSettings.WebApiURLUser);

                //Putting our username and password in a model because I want them going over in the body. 
                AuthenticateModel model = new AuthenticateModel();
                model = new AuthenticateModel();
                model.Username = userName;
                model.Password = password;

                //Put them over in a post because I feel it works better than a get method. 
                var responseTask = client.PostAsJsonAsync("authenticate", model);
                responseTask.Wait();

                //Let's look at the response. 
                var authResult = responseTask.Result;

                if (!authResult.IsSuccessStatusCode)
                {
                    resultofclient = SignInResult.Failed;
                }
                else
                { 
                    //Great we got a success now let's populate some objects.
                    resultofclient = SignInResult.Success;
                    
                    //Let's get the user login. 
                    var userLogin = await authResult.Content.ReadAsAsync<AuthenticateModel>();

                    //Lets populate the cookies and user principal.
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Email, userLogin.Username));
                    claims.Add(new Claim(ClaimTypes.Role, userLogin.Role));
                    claims.Add(new Claim(ClaimTypes.Hash, userLogin.Token));

                    //Need to create a Claims Identity. 
                    var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Let's set the role. 
                    userIdentity.AddClaim(new Claim(ClaimTypes.Role, userLogin.Role));

                    // Let's set some authentication properties. 
                    var authProperties = new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.Now.AddMinutes(this.externalAppSettings.MinutesToPersistUser),
                    };

                    // Let's get the claims prinicipal. 
                    var principal = new ClaimsPrincipal(userIdentity);

                    //Let's sign in with the httpcontext because I am using cookies. 
                    await this._contextAccessor.HttpContext.SignInAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                        principal: principal, properties: authProperties);



                }
                
            }

            return resultofclient; 
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
