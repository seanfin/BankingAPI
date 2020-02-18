using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Formatting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;



using Banking.Core.Utils;
using Banking.Core.Models;


namespace Banking.Web.UI.Controllers
{

    public class AccountController : Controller
    {
        private readonly ExternalAppSettings _appSettings;

        private readonly SignInManager<IdentityUser> _signInManager;


        public AccountController(IOptions<ExternalAppSettings> appSettings, SignInManager<IdentityUser> signInManager)
        {
          
            _appSettings = AppSettingHelper.GetExternalApplicationConfiguration();
            this._signInManager = signInManager;

        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AuthenticateModel authenticateModel)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this._appSettings.WebApiURLUser);

                //HTTP GET
                var responseTask = client.PostAsJsonAsync("authenticate", authenticateModel);
                responseTask.Wait();

                var result = responseTask.Result;
                if (!result.IsSuccessStatusCode)
                {

                    ModelState.AddModelError("", "User not found");
                    return View();
                }


                var userLogin = await result.Content.ReadAsAsync<UserLogin>();

                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, userLogin.FirstName));
                claims.Add(new Claim(ClaimTypes.Sid, userLogin.Id.ToString()));



                //CookieAuthenticationDefaults.AuthenticationScheme
                var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                //identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));

                //foreach (var role in user.Roles)
                //{
                userIdentity.AddClaim(new Claim(ClaimTypes.Role, userLogin.Role));

                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20),
                    IsPersistent = true,

                };

               
                var principal = new ClaimsPrincipal(userIdentity);
                //await HttpContext.SignOutAsync();
                //var appUser = await this._signInManager.UserManager.GetUserAsync(principal);
                await HttpContext.SignInAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                    principal: principal);

               
               



                //await AuthenticationHttpContextExtensions.SignInAsync(HttpContext, principal);

                //MarshalByRefObject

                //    HttpContext.User.AddIdentity(userIdentity);




                return RedirectToAction("Index", "Home");

            }

        }

        



        public async Task<IActionResult> Logout()
        {
            //await HttpContext.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}