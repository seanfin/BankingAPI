using System;




namespace BankingConsole
{
    class Program
    {

        static bool exitApplication = false;

        


        Program()
        {
       

        }


        static void Main(string[] args)
        {

            BankingConsoleActions actions = new BankingConsoleActions();

            while (!exitApplication)
            {
                 

                Console.WriteLine("Welcome to BankingConsole!!");
                Console.WriteLine("Please Input a Command.");
                Console.WriteLine("Type Helper to get a List of Commands");

                var command = Console.ReadLine();
                try
                {

                    if (command.Trim().ToLower() == BankingConsoleActions.Command_Login.Trim().ToLower())
                    {
                        actions.Login();
                    }
                    else if (command.Trim().ToLower() == BankingConsoleActions.Command_CreateAccount.Trim().ToLower())
                    {
                        actions.CreateNewAccount();
                    }
                    else if (command.Trim().ToLower() == BankingConsoleActions.Command_PostTransaction.Trim().ToLower())
                    {
                        actions.PostTransaction();
                    }
                    else if (command.Trim().ToLower() == BankingConsoleActions.Command_GetAccountBalance.Trim().ToLower())
                    {
                        actions.GetCurrentBalance();
                    }
                    else if (command.Trim().ToLower() == BankingConsoleActions.Command_GetTransactionHistory.Trim().ToLower())
                    {
                        actions.GetTransactionHistory();
                    }
                    else if (command.Trim().ToLower() == BankingConsoleActions.Command_Helper.Trim().ToLower())
                    {
                        actions.DisplayHelper();
                    }
                    else if (command.Trim().ToLower() == BankingConsoleActions.Command_Logout.Trim().ToLower())
                    {
                        exitApplication = actions.LogOut();
                    }

                }
                catch(Exception ex )
                {
                    Console.WriteLine(ex.Message);
                }
            }






        }

       

       





    }
}
