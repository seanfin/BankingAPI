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
    

    public class ProfileService : IProfileService
    {
        protected MemoryCache _userProfileCache = new MemoryCache("userProfile");

        private readonly AppSettings _appSettings;

        public ProfileService(IOptions<AppSettings> appSettings) 
        {
            this._appSettings = appSettings.Value;

            // users hardcoded for simplicity, store in a db with hashed passwords in production applications
            ProfileInformation profile1 = new ProfileInformation { FirstName = "Gabe", LastName = "Smith", Username = "GSmight@Avengers.com" };
            ProfileInformation profile2 = new ProfileInformation { FirstName = "Janet", LastName = "Markson", Username = "Janet@Pizza.com" };

            List<int> accountNumbers1 = new List<int>();
            accountNumbers1.Add(99868786);
            accountNumbers1.Add(584752341);

            profile2.BankAccountNumbers = accountNumbers1;

            //Add the logins into the cache to start off. 
            AddProfileInformation(profile1);
            AddProfileInformation(profile2);


        }

        /// <summary>
        /// This allows us to add profiles into the system. 
        /// </summary>
        /// <param name="profileInformation">The user login to add into the system.</param>
        /// <returns>Returns a user login populated with the ID after it has been added to the DB/Cache.</returns>
        public ProfileInformation AddProfileInformation(ProfileInformation profileInformation)
        {

            if (string.IsNullOrEmpty(profileInformation.Username.Trim()))
            {
                throw new Exception("In order to add user to the database we need a username");
            }



            lock (this)
            {

                profileInformation.Id = Guid.NewGuid();

                //Create a cache policy so that it will expire eventually.
                var policy = new CacheItemPolicy()
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(this._appSettings.CacheExpirationInMinutes)
                };

                //let's create a cache item. 
                var itemToCache = new CacheItem(profileInformation.Username, profileInformation);

                //Now lets set the cache container to have the new data. 
                this._userProfileCache.Set(itemToCache, policy);
            }

            var clonedProfile = SecurityHelper.DeepClone(profileInformation);


            return clonedProfile;
        }

        /// <summary>
        /// This retrieves all of the profiles that are in our system.
        /// </summary>
        /// <returns>This returns an object with all of the users that are in the system.</returns>
        public IEnumerable<ProfileInformation> GetAllProfileInformation()
        {
            //Create a list from the 
            List<ProfileInformation> logins = new List<ProfileInformation>();

            lock (this)
            {


                //Get a reference to the cache
                var cacheContainer = this._userProfileCache;

                //Get an enumerator.
                IDictionaryEnumerator cacheEnumerator = (IDictionaryEnumerator)((IEnumerable)this._userProfileCache).GetEnumerator();

                //Loop through and get the logins.
                while (cacheEnumerator.MoveNext())
                {
                    ProfileInformation existingLogin = (ProfileInformation)cacheEnumerator.Value;
                    var cloneExistingLogin = SecurityHelper.DeepClone(existingLogin);
                    logins.Add(cloneExistingLogin);
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
        public ProfileInformation GetByIDProfileInformation(Guid id)
        {
            //Let's get all of the users.
            var users = GetAllProfileInformation();

            //Let's look for the ones with the correct IDs.
            var user = users.FirstOrDefault(x => x.Id == id);

            //return the user without the password.
            return user;
        }


        /// <summary>
        /// Retrieves an user by their ID.
        /// </summary>
        /// <param name="id">A unique identifier for the user.</param>
        /// <returns>Returns the user login.</returns>
        public ProfileInformation GetProfileInformationByEmail(string userName)
        {
            //Let's get all of the users.
            var users = GetAllProfileInformation();

            //Let's look for the ones with the correct IDs.
            var profileInformation = users.FirstOrDefault(x => x.Username.Trim().ToLower() == userName.Trim().ToLower());

            //return the user without the password.
            return profileInformation;
        }


    }
}
