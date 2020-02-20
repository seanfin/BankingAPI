using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Banking.Core.Interfaces;


namespace BankingAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        IProfileService _profileService;

        public ProfileController(IProfileService profileservice)
        {
            this._profileService = profileservice;

        }


     
        /// <summary>
        /// Retrieves the user profile by the username.
        /// </summary>
        /// <param name="userName">the username.</param>
        /// <returns>The user profile embeded in an action.</returns>
        [HttpPost("getprofileinfobyusername")]
        public IActionResult GetUserGrofile([FromBody] string userName)
        {
            var profileInfo = this._profileService.GetProfileInformationByEmail(userName);

            return Ok(profileInfo);
        }

        
    }
}
