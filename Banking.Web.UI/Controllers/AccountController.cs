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
        public IActionResult CreateAccount()
        {
            return View();
        }


        [HttpPost]
        public IActionResult CreateAccount (AuthenticateModel authenticateModel)
        {

            using (var client = new HttpClient())
            {
                //Getting access to our web api. 
                client.BaseAddress = new Uri(this._appSettings.WebApiURLUser);

                //Putting our username and password in a model because I want them going over in the body. 
               

                //Put them over in a post because I feel it works better than a get method. 
                var responseTask = client.PostAsJsonAsync("createauthenicationaccount", authenticateModel);
                responseTask.Wait();


                //Let's look at the response. 
                var authResult = responseTask.Result;

                if (!authResult.IsSuccessStatusCode)
                {
                    
                    throw new Exception("There was an issue while creating your account");
                }
                
                   
                

            }

            return RedirectToAction("Index", "Home");
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


                
                //Getting access to our web api. 
                client.BaseAddress = new Uri(this._appSettings.WebApiURLUser);

                //Putting our username and password in a model because I want them going over in the body. 
             

                //Put them over in a post because I feel it works better than a get method. 
                var responseTask = client.PostAsJsonAsync("authenticate", authenticateModel);
                responseTask.Wait();

                //Let's look at the response. 
                var authResult = responseTask.Result;

                if (!authResult.IsSuccessStatusCode)
                {
                    throw new Exception("There was an issue when attempting to login");
                }
                else
                {                    

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
                        ExpiresUtc = DateTime.Now.AddMinutes(this._appSettings.MinutesToPersistUser),
                    };

                    // Let's get the claims prinicipal. 
                    var principal = new ClaimsPrincipal(userIdentity);

                    //Let's sign in with the httpcontext because I am using cookies. 
                    await HttpContext.SignInAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                        principal: principal, properties: authProperties);



                }

            }

            return RedirectToAction("Index", "Home");



        }

        public ActionResult Validate()
        {

            string d = "w";

            //var _admin = db.Admins.Where(s => s.Email == admin.Email);
            //if (_admin.Any())
            //{
            //    if (_admin.Where(s => s.Password == admin.Password).Any())
            //    {

            //        return Json(new { status = true, message = "Login Successfull!" });
            //    }
            //    else
            //    {
            //        return Json(new { status = false, message = "Invalid Password!" });
            //    }
            //}
            //else
            //{
            //    return Json(new { status = false, message = "Invalid Email!" });
            //}

            return View();

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