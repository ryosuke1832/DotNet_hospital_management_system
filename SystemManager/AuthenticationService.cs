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

            // Display test user information if sample data exists
            DisplayTestUserInfo();

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

        /// <summary>
        /// Display test user information for easy testing
        /// </summary>
        private void DisplayTestUserInfo()
        {
            // Check if we have sample data (basic check)
            if (dataManager.Administrators.Any(a => a.Password == "admin123") ||
                dataManager.Doctors.Any(d => d.Password == "doctor123") ||
                dataManager.Patients.Any(p => p.Password == "patient123"))
            {
                Console.WriteLine("=== TEST USERS (for demonstration) ===");

                // Display Administrator info
                var admin = dataManager.Administrators.FirstOrDefault(a => a.Password == "admin123");
                if (admin != null)
                {
                    Console.WriteLine($"Administrator - ID: {admin.Id}, Password: admin123");
                }

                // Display Doctor info
                var doctor = dataManager.Doctors.FirstOrDefault(d => d.Password == "doctor123");
                if (doctor != null)
                {
                    Console.WriteLine($"Doctor - ID: {doctor.Id}, Password: doctor123");
                }

                // Display Patient info
                var patient = dataManager.Patients.FirstOrDefault(p => p.Password == "patient123");
                if (patient != null)
                {
                    Console.WriteLine($"Patient - ID: {patient.Id}, Password: patient123");
                }

                Console.WriteLine("======================================");
            }
        }
    }
}