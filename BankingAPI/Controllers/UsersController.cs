using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

using Banking.Core.Interfaces;
using Banking.Core.Models;
using Banking.Core.Utils;
using Banking.Core.Helper;
using Banking.Core.Enums;


using System;


namespace BankingAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateModel authenticationModel)
        {
            var user = _userService.Authenticate(authenticationModel);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            //user = SecurityHelper.RemovePassword(user);

            

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("getprofileinfobyusername")]
        public IActionResult Authenticate([FromBody] string userName)
        {
            var profileInfo = _userService.GetProfileInformationByEmail(userName);

            return Ok(profileInfo);
        }


        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAllAuthenticationModels();

            List<AuthenticateModel> cleanUsers = new List<AuthenticateModel>();
            cleanUsers.ForEach(model => SecurityHelper.RemovePassword(model));

            return Ok(cleanUsers);

            

        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            // only allow admins to access other user records
            
            if (!User.IsInRole(Role.Admin))
                return Forbid();

            var user = _userService.GetByIdAuthenticationModel(id);

            if (user == null)
                return NotFound();

            user = SecurityHelper.RemovePassword(user);

            return Ok(user);
        }
    }
}