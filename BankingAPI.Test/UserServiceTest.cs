using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Microsoft.Extensions.Options;
using Banking.Core;
using System.Collections.Generic;

using Banking.Core.Models;
using Banking.Core.Helper;
using Banking.Core.Enums;
using Banking.Core.Utils;



namespace Banking.Core.Test
{
    /// <summary>
    /// These are the tests for the UserService implementation.
    /// </summary>
    [TestClass]
    public class UserServiceTest
    {
        private AppSettings _appSettings;

        [TestInitialize()]
        public void Startup()
        {
            _appSettings = AppSettingHelper.GetApplicationConfiguration();
        }


        [TestMethod]
        public void UserService_GetAll()
        {
            //Create the options and the User Service
            IOptions<AppSettings> settings = Options.Create(this._appSettings);
            UserService userService = new UserService(settings);
            
            //Get all of the Users.
            var allUsers = userService.GetAllAuthenticationModels();

            //Check to see that the default users are there.
            Assert.IsTrue(allUsers.ToArray().Length > 0);

            //Make sure it does not come back null.
            Assert.IsNotNull(allUsers);
        }

        [TestMethod]
        public void UserService_AddAccount()
        {
            //The account number we will be using. 
            int accountNumber1 = 123456;

            ProfileInformation userLogin1 = new ProfileInformation();
            userLogin1.FirstName = "Peter";
            userLogin1.LastName = "Parker";
            userLogin1.Username = "SpiderMan@Avengers.com";

            List<int> accountNumbers1 = new List<int>();
            accountNumbers1.Add(accountNumber1);

            userLogin1.BankAccountNumbers = accountNumbers1.ToArray();


            //The account number we will be using. 
            int accountNumber2 = 102356;

            ProfileInformation userLogin2 = new ProfileInformation();
            userLogin2.FirstName = "Bruce";
            userLogin2.LastName = "Wayne";
            userLogin2.Username = "Batman@DC.com";
            
            List<int> accountNumbers2 = new List<int>();
            accountNumbers2.Add(accountNumber2);
            
            userLogin2.BankAccountNumbers = accountNumbers2.ToArray();
            


            //Create the options and the User Service
            IOptions<AppSettings> settings = Options.Create(this._appSettings);
            UserService userService = new UserService(settings);

            //Let's add the first user.
            var user1AfterAdded = userService.AddProfileInformation(userLogin1);

            //Check to see if the user has been added. 
            Assert.IsNotNull(userService.GetByIDProfileInformation(user1AfterAdded.Id));

            //Let's add the second user.
            var user2AfterAdded = userService.AddProfileInformation(userLogin2);

            //Check to see if the user has been added. 
            Assert.IsNotNull(userService.GetByIDProfileInformation(user2AfterAdded.Id));
            
        }


        [TestMethod]
        public void UserService_GetByID()
        {
            //The account number we will be using. 
            int accountNumber1 = 889786;

            ProfileInformation userLogin1 = new ProfileInformation();
            userLogin1.FirstName = "Charles";
            userLogin1.LastName = "Xavier";
            userLogin1.Username = "ProfessorX@XMen.com";

            List<int> bankAccountNumber1 = new List<int>();
            bankAccountNumber1.Add(accountNumber1);

            userLogin1.BankAccountNumbers = bankAccountNumber1.ToArray();



            //Create the options and the User Service
            IOptions<AppSettings> settings = Options.Create(this._appSettings);
            UserService userService = new UserService(settings);

            //Let's add the first user.
            var user1AfterAdded = userService.AddProfileInformation(userLogin1);

            //Check to see if the user has been added. 
            Assert.IsNotNull(userService.GetByIDProfileInformation(user1AfterAdded.Id));
            
        }


        [TestMethod]
        public void BankTransaction_Authenticate()
        {
            //The account number we will be using. 
            int accountNumber1 = 101010;
            string username1Password = "XmenSuck@Badguys.com";

            AuthenticateModel authenticationModel1 = new AuthenticateModel();
            authenticationModel1.Username = "Magento@Avengers.com";
            authenticationModel1.Password = username1Password;
            
            authenticationModel1.Role = Role.User;

            //The account number we will be using. 

            string username2Password = "ShapshiftingRocks@";

            AuthenticateModel authenticationModel2 = new AuthenticateModel();
            
            authenticationModel2.Username = "Mystique@Badguys.com";
            authenticationModel2.Password = username2Password;
            authenticationModel2.Role = Role.User;
            

            //Create the options and the User Service
            IOptions<AppSettings> settings = Options.Create(this._appSettings);
            UserService userService = new UserService(settings);

            //Let's add the first user.
            var user1AfterAdded = userService.AddAuthenticationModel(authenticationModel1);

            //Check to see if the user has been added. 
            Assert.IsNotNull(userService.GetByIdAuthenticationModel(user1AfterAdded.Id));

            //Let's add the second user.
            var user2AfterAdded = userService.AddAuthenticationModel(authenticationModel2);

            //Check to see if the user has been added. 
            Assert.IsNotNull(userService.GetByIdAuthenticationModel(user2AfterAdded.Id));


            //Let's see that the users are getting authenticated. 
            var userLogin1AfterAuthenication = userService.Authenticate(authenticationModel1);

            //Let's check the token
            Assert.IsNotNull(userLogin1AfterAuthenication.Token);

            //Let's see that the users are getting authenticated. 
            var userLogin2AfterAuthenication = userService.Authenticate(authenticationModel2);

            //Let's check the token
            Assert.IsNotNull(userLogin2AfterAuthenication.Token);


        }


    }
}
