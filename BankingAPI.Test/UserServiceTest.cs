using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Microsoft.Extensions.Options;
using Banking.Core;
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
            var allUsers = userService.GetAll();

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

            UserLogin userLogin1 = new UserLogin();
            userLogin1.FirstName = "Peter";
            userLogin1.LastName = "Parker";
            userLogin1.Username = "SpiderMan@Avengers.com";
            userLogin1.Password = "SpideyRules@";
            userLogin1.UserBankAccounts.Add(accountNumber1);
            userLogin1.Role = Role.User;

            //The account number we will be using. 
            int accountNumber2 = 102356;

            UserLogin userLogin2 = new UserLogin();
            userLogin2.FirstName = "Bruce";
            userLogin2.LastName = "Wayne";
            userLogin2.Username = "Batman@DC.com";
            userLogin2.Password = "CatwomanLove@";
            userLogin2.UserBankAccounts.Add(accountNumber2);
            userLogin2.Role = Role.User;


            //Create the options and the User Service
            IOptions<AppSettings> settings = Options.Create(this._appSettings);
            UserService userService = new UserService(settings);

            //Let's add the first user.
            var user1AfterAdded = userService.AddUser(userLogin1);

            //Check to see if the user has been added. 
            Assert.IsNotNull(userService.GetById(user1AfterAdded.Id));

            //Let's add the second user.
            var user2AfterAdded = userService.AddUser(userLogin2);

            //Check to see if the user has been added. 
            Assert.IsNotNull(userService.GetById(user2AfterAdded.Id));
            
        }


        [TestMethod]
        public void UserService_GetByID()
        {
            //The account number we will be using. 
            int accountNumber1 = 889786;

            UserLogin userLogin1 = new UserLogin();
            userLogin1.FirstName = "Charles";
            userLogin1.LastName = "Xavier";
            userLogin1.Username = "ProfessorX@XMen.com";
            userLogin1.Password = "JeanGrey123";
            userLogin1.UserBankAccounts.Add(accountNumber1);
            userLogin1.Role = Role.User;

            
            //Create the options and the User Service
            IOptions<AppSettings> settings = Options.Create(this._appSettings);
            UserService userService = new UserService(settings);

            //Let's add the first user.
            var user1AfterAdded = userService.AddUser(userLogin1);

            //Check to see if the user has been added. 
            Assert.IsNotNull(userService.GetById(user1AfterAdded.Id));
            
        }


        [TestMethod]
        public void BankTransaction_Authenticate()
        {
            //The account number we will be using. 
            int accountNumber1 = 101010;
            string username1Password = "XmenSuck@Badguys.com";

            UserLogin userLogin1 = new UserLogin();
            userLogin1.FirstName = "Erik";
            userLogin1.LastName = "Lehnsherr";
            userLogin1.Username = "Magento@Avengers.com";
            userLogin1.Password = username1Password;
            userLogin1.UserBankAccounts.Add(accountNumber1);
            userLogin1.Role = Role.User;

            //The account number we will be using. 
            int accountNumber2 = 9876588;
            string username2Password = "ShapshiftingRocks@";

            UserLogin userLogin2 = new UserLogin();
            userLogin2.FirstName = "Raven";
            userLogin2.LastName = "Darkholme";
            userLogin2.Username = "Mystique@Badguys.com";
            userLogin2.Password = username2Password;
            userLogin2.UserBankAccounts.Add(accountNumber2);
            userLogin2.Role = Role.User;


            //Create the options and the User Service
            IOptions<AppSettings> settings = Options.Create(this._appSettings);
            UserService userService = new UserService(settings);

            //Let's add the first user.
            var user1AfterAdded = userService.AddUser(userLogin1);

            //Check to see if the user has been added. 
            Assert.IsNotNull(userService.GetById(user1AfterAdded.Id));

            //Let's add the second user.
            var user2AfterAdded = userService.AddUser(userLogin2);

            //Check to see if the user has been added. 
            Assert.IsNotNull(userService.GetById(user2AfterAdded.Id));


            //Let's see that the users are getting authenticated. 
            var userLogin1AfterAuthenication = userService.Authenticate(userLogin1.Username, username1Password);

            //Let's check the token
            Assert.IsNotNull(userLogin1AfterAuthenication.Token);

            //Let's see that the users are getting authenticated. 
            var userLogin2AfterAuthenication = userService.Authenticate(userLogin2.Username, username2Password);

            //Let's check the token
            Assert.IsNotNull(userLogin2AfterAuthenication.Token);


        }


    }
}
