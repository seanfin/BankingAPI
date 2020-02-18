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


using Banking.Core.Utils;
using Banking.Core.Models;


namespace Banking.Web.UI.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly ExternalAppSettings _appSettings;

        public AccountController(IOptions<ExternalAppSettings> appSettings)
        {

            _appSettings = AppSettingHelper.GetExternalApplicationConfiguration();
            
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
                


                    var userIdentity = new ClaimsIdentity(claims, "CustomApiKeyAuth");

                //identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));

                //foreach (var role in user.Roles)
                //{
                userIdentity.AddClaim(new Claim(ClaimTypes.Role, userLogin.Role));
                //}

                var principal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignOutAsync();
                await HttpContext.SignInAsync(principal);

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