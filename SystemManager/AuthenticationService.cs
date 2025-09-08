using Assignment1_hospital_management_system.Models;
using Assignment1_hospital_management_system.Utilities;
using System;
using System.Linq;

namespace Assignment1_hospital_management_system.SystemManager
{
    /// <summary>
    /// 簡略化された認証サービス
    /// </summary>
    public class AuthenticationService
    {
        private DataManager dataManager;

        public AuthenticationService(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }

        public bool HandleLogin(MenuController menuController)
        {
            Utils.DisplayHeader("Login");
            DisplayAvailableAccounts();

            Console.WriteLine("\n=== Login ===");

            // ユーザーID入力
            string userIdInput = Utils.GetStringInput("User ID (enter 'exit' to quit): ");
            if (IsExitCommand(userIdInput)) return ConfirmExit();

            if (!int.TryParse(userIdInput, out int userId))
            {
                Console.WriteLine("Invalid User ID. Please enter a numeric value.");
                Utils.PressAnyKeyToContinue();
                return false;
            }

            // パスワード入力
            string password = Utils.GetPasswordInputWithExit("Password (enter 'exit' to quit): ");
            if (IsExitCommand(password)) return ConfirmExit();

            // 認証処理
            User currentUser = dataManager.FindUser(userId, password);
            if (currentUser != null)
            {
                Console.WriteLine("Authentication successful! Logging in...");
                System.Threading.Thread.Sleep(1000);
                return menuController.ShowUserMenu(currentUser);
            }
            else
            {
                Console.WriteLine("Invalid ID or password. Please try again.");
                Utils.PressAnyKeyToContinue();
                return false;
            }
        }

        private void DisplayAvailableAccounts()
        {
            Console.WriteLine("=== Available Test Accounts ===");

            DisplayUserGroup("Administrator", dataManager.Administrators);
            DisplayUserGroup("Doctor", dataManager.Doctors);
            DisplayUserGroup("Patient", dataManager.Patients);
            DisplayUserGroup("Receptionist", dataManager.Receptionists);

            Console.WriteLine("================================");
        }

        private void DisplayUserGroup<T>(string groupName, System.Collections.Generic.List<T> users) where T : User
        {
            if (users.Any())
            {
                Console.WriteLine($"【{groupName} Accounts】");
                foreach (var user in users)
                {
                    string prefix = user is Doctor ? "Dr. " : "";
                    Console.WriteLine($"  ID: {user.Id} | Name: {prefix}{user.FirstName} {user.LastName} | Password: {user.Password}");
                }
                Console.WriteLine();
            }
        }

        private bool IsExitCommand(string input) =>
            !string.IsNullOrWhiteSpace(input) &&
            new[] { "exit", "quit", "q" }.Contains(input.Trim().ToLower());

        private bool ConfirmExit()
        {
            Console.WriteLine("\nDo you want to exit the system?");
            string confirmation = Utils.GetStringInput("Enter 'y' or 'yes' to exit: ");

            if (new[] { "y", "yes" }.Contains(confirmation.Trim().ToLower()))
            {
                Console.WriteLine("Exiting the system...");
                System.Threading.Thread.Sleep(1000);
                return true;
            }
            else
            {
                Console.WriteLine("Returning to login screen...");
                System.Threading.Thread.Sleep(1000);
                return false;
            }
        }
    }
}