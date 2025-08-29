using Assignment1_hospital_management_system.Models;
using Assignment1_hospital_management_system.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1_hospital_management_system.SystemManager
{
    /// <summary>
    /// Handles user authentication and login process
    /// </summary>
    public class AuthenticationService
    {
        private DataManager dataManager;

        public AuthenticationService(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }

        /// <summary>
        /// Handle the login process and redirect to appropriate menu
        /// </summary>
        public bool HandleLogin(MenuController menuController)
        {
            Utils.DisplayHeader("Login");
            Console.WriteLine();

            int userId = Utils.GetIntegerInput("ID: ");
            string password = Utils.GetPasswordInput("Password: ");

            User currentUser = dataManager.FindUser(userId, password);

            if (currentUser != null)
            {
                Console.WriteLine("Valid Credentials");
                System.Threading.Thread.Sleep(1000);

                // Show appropriate menu based on user type
                return menuController.ShowUserMenu(currentUser);
            }
            else
            {
                Console.WriteLine("Invalid credentials. Please try again.");
                Utils.PressAnyKeyToContinue();
                return false;
            }
        }
    }
}
