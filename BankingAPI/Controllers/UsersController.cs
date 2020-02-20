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

        /// <summary>
        /// Authentications the user and provides a token.
        /// </summary>
        /// <param name="authenticationModel"></param>
        /// <returns></returns>
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

       

        /// <summary>
        /// Retrieves all of the users. 
        /// </summary>
        /// <returns>Returns a  list of all of the users</returns>
        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAllAuthenticationModels();

            List<AuthenticateModel> cleanUsers = new List<AuthenticateModel>();
            cleanUsers.ForEach(model => SecurityHelper.RemovePassword(model));

            return Ok(cleanUsers);

            

        }

        /// <summary>
        /// Create a authentication record essentially registering a user. 
        /// </summary>
        /// <param name="authenticateModel">The username and password.</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("createauthenicationaccount")]
        public IActionResult CreateAcount(AuthenticateModel authenticateModel)
        {
            var addModel = _userService.AddAuthenticationModel(authenticateModel);

            addModel = SecurityHelper.RemovePassword(addModel);

            return Ok(addModel);



        }

        /// <summary>
        /// Retrieves autentication model by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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