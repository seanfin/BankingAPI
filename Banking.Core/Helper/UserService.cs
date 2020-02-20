using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Runtime.Caching;


using Banking.Core.Interfaces;
using Banking.Core.Models;
using Banking.Core.Utils;
using Banking.Core.Enums;

namespace Banking.Core.Helper
{
    public class UserService : IUserService
    {

        protected MemoryCache userLogin = new MemoryCache("UserLogin");
        

        private readonly AppSettings _appSettings;

        /// <summary>
        /// This is the implementation that allows us to be able to authenticate and authorize users into the system. 
        /// </summary>
        /// <param name="appSettings"></param>
        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;


            // users hardcoded for simplicity, store in a db with hashed passwords in production applications
            AuthenticateModel login1 = new AuthenticateModel { Username = "GSmight@Avengers.com", Password = "AvengersRule@", Role = Role.Admin };
            AuthenticateModel login2 = new AuthenticateModel { Username = "Janet@Pizza.com", Password = "PizzaTime", Role = Role.User };

            //Add the logins into the cache to start off. 
            AddAuthenticationModel(login1);
            AddAuthenticationModel(login2);



           
           


            
        }

        /// <summary>
        /// This allows us to add users into the system. 
        /// </summary>
        /// <param name="authenticationModel">The user login to add into the system.</param>
        /// <returns>Returns a user login populated with the ID after it has been added to the DB/Cache.</returns>
        public AuthenticateModel AddAuthenticationModel(AuthenticateModel authenticationModel)
        {

            if (string.IsNullOrEmpty(authenticationModel.Username.Trim()))
            {
                throw new Exception("In order to add user to the database we need a username");
            }
            else if (string.IsNullOrEmpty(authenticationModel.Password.Trim()))
            {
                throw new Exception("In order to add user to the database we need a password");
            }

                       
            lock (this)
            {

                //Let's check to see if this username already exists. 

                if (this.userLogin.Get(authenticationModel.Username) != null)
                {
                    throw new Exception("The username you are trying to use already exists.");
                }

                authenticationModel.Id = Guid.NewGuid();

                //Create a cache policy so that it will expire eventually.
                var policy = new CacheItemPolicy()
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(this._appSettings.CacheExpirationInMinutes)
                };

                //let's create a cache item. 
                var itemToCache = new CacheItem(authenticationModel.Username, authenticationModel);

                //Now lets set the cache container to have the new data. 
                this.userLogin.Set(itemToCache, policy);
            }

            var clonedModel = SecurityHelper.DeepClone(authenticationModel);

            return clonedModel;
        }

               

        /// <summary>
        /// Allows us to authenticate the user with the information that we are provided. 
        /// </summary>
        /// <param name="username">The user name we are provided.</param>
        /// <param name="password">The password we are provided with</param>
        /// <returns>Provide a userlogin object populated with a token.</returns>
        public AuthenticateModel Authenticate(AuthenticateModel authenticateModel)
        {
            if (string.IsNullOrEmpty(authenticateModel.Username.Trim()))
            {
                throw new Exception("In order to Authenticate user we need a username");
            }
            else if (string.IsNullOrEmpty(authenticateModel.Password.Trim()))
            {
                throw new Exception("In order to Authenticate user we need a password");
            }

            AuthenticateModel user = null;

            lock (this)
            {

                //Let's get all of the users.
                var users = GetAllAuthenticationModels();


                user = users.SingleOrDefault(x => x.Username.ToLower().Trim() == authenticateModel.Username.ToLower().Trim());
                
                //do the passwords match?
                if(user == null || user.Password.ToLower().Trim() != authenticateModel.Password.ToLower().Trim())
                {
                    return null;
                }

                //// return null if user not found
                //if (user == null)
                //    return null;

                // authentication successful so generate jwt token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                user.Token = tokenHandler.WriteToken(token);
                                
            }

            return user;
        }

        /// <summary>
        /// This retrieves all of the users that are in our system.
        /// </summary>
        /// <returns>This returns an object with all of the users that are in the system.</returns>
        public IEnumerable<AuthenticateModel> GetAllAuthenticationModels()
        {
            //Create a list from the 
            List<AuthenticateModel> logins = new List<AuthenticateModel>();

            lock (this)
            {


                //Get a reference to the cache
                var cacheContainer = this.userLogin;

                //Get an enumerator.
                IDictionaryEnumerator cacheEnumerator = (IDictionaryEnumerator)((IEnumerable)this.userLogin).GetEnumerator();

                //Loop through and get the logins.
                while (cacheEnumerator.MoveNext())
                {
                    AuthenticateModel existingLogin = (AuthenticateModel)cacheEnumerator.Value;
                    var cloneExistingLogin = SecurityHelper.DeepClone(existingLogin);
                    logins.Add(existingLogin);
                }

            }

            //Clean out the passwords
            
            return logins;
        }

       


        /// <summary>
        /// Retrieves an user by their ID.
        /// </summary>
        /// <param name="id">A unique identifier for the user.</param>
        /// <returns>Returns the user login.</returns>
        public AuthenticateModel GetByIdAuthenticationModel(Guid id)
        {
            //Let's get all of the users.
            var users = GetAllAuthenticationModels();

            //Let's look for the ones with the correct IDs.
            var user = users.FirstOrDefault(x => x.Id == id);

            //return the user without the password.
            return user;
        }


      


    }
}
