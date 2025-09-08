using Assignment1_hospital_management_system.Models;
using Assignment1_hospital_management_system.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment1_hospital_management_system.SystemManager
{
    /// <summary>
    /// Handles user authentication and login process with exit option
    /// </summary>
    public class AuthenticationService
    {
        private DataManager dataManager;

        public AuthenticationService(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }

        /// <summary>
        /// Handle the login process with exit option
        /// </summary>
        public bool HandleLogin(MenuController menuController)
        {
            Utils.DisplayHeader("Login");

            Console.WriteLine();
            Console.WriteLine("Login Instructions:");
            Console.WriteLine("1. Please enter the test user ID from below");
            Console.WriteLine("2. Enter the corresponding password");
            Console.WriteLine("3. Password will be displayed as '*' on screen");
            Console.WriteLine("4. Enter 'exit' to quit the system");
            Console.WriteLine();

            // Display available accounts information
            DisplayAvailableAccounts();

            Console.WriteLine();
            Console.WriteLine("=== Login ===");

            // User ID input (with exit check)
            string userIdInput = Utils.GetStringInput("User ID (enter 'exit' to quit): ");

            // Exit command check
            if (IsExitCommand(userIdInput))
            {
                return ConfirmExit();
            }

            // ID numeric conversion check
            if (!int.TryParse(userIdInput, out int userId))
            {
                Console.WriteLine("Invalid User ID. Please enter a numeric value.");
                Utils.PressAnyKeyToContinue();
                return false; // Return to login screen
            }

            // Password input (with exit check)
            string password = Utils.GetPasswordInputWithExit("Password (enter 'exit' to quit): ");

            // Exit check for password input
            if (IsExitCommand(password))
            {
                return ConfirmExit();
            }

            // Authentication process
            User currentUser = dataManager.FindUser(userId, password);

            if (currentUser != null)
            {
                Console.WriteLine("Authentication successful! Logging into the system...");
                System.Threading.Thread.Sleep(1500);

                // Display appropriate menu
                return menuController.ShowUserMenu(currentUser);
            }
            else
            {
                Console.WriteLine("Invalid ID or password. Please try again.");
                Utils.PressAnyKeyToContinue();
                return false; // Return to login screen
            }
        }

        /// <summary>
        /// Display available account information
        /// </summary>
        private void DisplayAvailableAccounts()
        {
            Console.WriteLine("=== Available Test Accounts ===");

            // Administrator information
            if (dataManager.Administrators.Count > 0)
            {
                Console.WriteLine("【Administrator Accounts】");
                foreach (var admin in dataManager.Administrators)
                {
                    Console.WriteLine($"  ID: {admin.Id} | Name: {admin.FirstName} {admin.LastName} | Password: {admin.Password}");
                }
                Console.WriteLine();
            }

            // Doctor information
            if (dataManager.Doctors.Count > 0)
            {
                Console.WriteLine("【Doctor Accounts】");
                foreach (var doctor in dataManager.Doctors)
                {
                    Console.WriteLine($"  ID: {doctor.Id} | Name: Dr. {doctor.FirstName} {doctor.LastName} | Password: {doctor.Password}");
                }
                Console.WriteLine();
            }

            // Patient information
            if (dataManager.Patients.Count > 0)
            {
                Console.WriteLine("【Patient Accounts】");
                foreach (var patient in dataManager.Patients)
                {
                    Console.WriteLine($"  ID: {patient.Id} | Name: {patient.FirstName} {patient.LastName} | Password: {patient.Password}");
                }
                Console.WriteLine();
            }

            // Receptionist information
            if (dataManager.Receptionists.Count > 0)
            {
                Console.WriteLine("【Receptionist Accounts】");
                foreach (var receptionist in dataManager.Receptionists)
                {
                    Console.WriteLine($"  ID: {receptionist.Id} | Name: {receptionist.FirstName} {receptionist.LastName} | Password: {receptionist.Password}");
                }
                Console.WriteLine();
            }

            Console.WriteLine("================================");
        }

        /// <summary>
        /// Check if input is an exit command
        /// </summary>
        private bool IsExitCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string normalized = input.Trim().ToLower();
            return normalized == "exit" ||
                   normalized == "quit" ||
                   normalized == "q";
        }

        /// <summary>
        /// Confirm exit
        /// </summary>
        private bool ConfirmExit()
        {
            Console.WriteLine();
            Console.WriteLine("Do you want to exit the system?");
            string confirmation = Utils.GetStringInput("Enter 'y' or 'yes' to exit: ");

            string normalized = confirmation.Trim().ToLower();
            if (normalized == "y" || normalized == "yes")
            {
                Console.WriteLine("Exiting the system...");
                System.Threading.Thread.Sleep(1000);
                return true; // Exit system
            }
            else
            {
                Console.WriteLine("Returning to login screen...");
                System.Threading.Thread.Sleep(1000);
                return false; // Return to login screen
            }
        }
    }
}