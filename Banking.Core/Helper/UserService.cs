using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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

        protected MemoryCache cache = new MemoryCache("CachingProvider");

        
        


        //TODO: move this to the appsettings.
        //Set the cache to expire in 30 minutes.
        private readonly int DEFAULT_CACHE_EXPIRATION_MINUTES = 30;

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            // users hardcoded for simplicity, store in a db with hashed passwords in production applications
            UserLogin userLogin1 = new UserLogin { FirstName = "Gabe", LastName = "Smith", Username = "GSmight@Avengers.com", Password = "AvengersRule@", Role = Role.Admin };
            UserLogin userLogin2 = new UserLogin { FirstName = "Janet", LastName = "Markson", Username = "Janet@Pizza.com", Password = "PizzaTime1229", Role = Role.User };

            _appSettings = appSettings.Value;
        }

        public UserLogin AddUser ( UserLogin userLogin)
        {

            if(string.IsNullOrEmpty(userLogin.Username.Trim()))
            {
                throw new Exception("In order to add user to the database we need a username");
            }
            else if (string.IsNullOrEmpty(userLogin.Password.Trim()))
            {
                throw new Exception("In order to add user to the database we need a password");
            }


                lock (this)
                {
                    userLogin.Id = Guid.NewGuid();                    

                    //Create a cache policy so that it will expire eventually.
                    var policy = new CacheItemPolicy()
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(DEFAULT_CACHE_EXPIRATION_MINUTES)
                    };

                    //let's create a cache item. 
                    var itemToCache = new CacheItem(userLogin.Username, userLogin);

                    //Now lets set the cache container to have the new data. 
                    this.cache.Set(itemToCache, policy);
                }



            return userLogin;
        }


        public UserLogin Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username.Trim()))
            {
                throw new Exception("In order to Authenticate user we need a username");
            }
            else if (string.IsNullOrEmpty(password.Trim()))
            {
                throw new Exception("In order to Authenticate user we need a password");
            }

            UserLogin user = null;

            lock (this)
            {

                //Let's get all of the users.
                var users = GetAll();


                user = users.SingleOrDefault(x => x.Username == username && x.Password == password);

                // return null if user not found
                if (user == null)
                    return null;

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

            return user.WithoutPassword();
        }

        public IEnumerable<UserLogin> GetAll()
        {
            //Create a list from the 
            List<UserLogin> logins = new List<UserLogin>();

            lock (this)
            {


                //Get a reference to the cache
                var cacheContainer = this.cache;

                

                //Loop through and get the logins.
                foreach(object userLogin in this.cache)
                {
                    //Type the login from object when it was in the cache. 
                    UserLogin existingLogin = (UserLogin)userLogin;

                   

                    //Add it to the array. 
                    logins.Add(existingLogin);
                }
                               
            }

            return logins.WithoutPasswords();
        }

        public UserLogin GetById(Guid id)
        {
            //Let's get all of the users.
            var users = GetAll();

            //Let's look for the ones with the correct IDs.
            var user = users.FirstOrDefault(x => x.Id == id);

            //return the user without the password.
            return user.WithoutPassword();
        }
    }
}
