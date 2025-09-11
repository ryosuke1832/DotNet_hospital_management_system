using Assignment1_hospital_management_system.Models;
using Assignment1_hospital_management_system.Utilities;


namespace Assignment1_hospital_management_system.SystemManager
{

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

            Console.WriteLine("\n=== Login ===");

            // inuput user ID
            string userIdInput = Utils.GetStringInput("User ID (enter 'exit' to quit): ");
            if (IsExitCommand(userIdInput)) return ConfirmExit();

            if (!int.TryParse(userIdInput, out int userId))
            {
                Console.WriteLine("Invalid User ID. Please enter a numeric value.");
                Utils.PressAnyKeyToContinue();
                return false;
            }

            // input password
            string password = Utils.GetPasswordInputWithExit("Password (enter 'exit' to quit): ");
            if (IsExitCommand(password)) return ConfirmExit();

            // authenticate user
            User currentUser = dataManager.FindUser(userId, password);
            if (currentUser != null)
            {
                Console.WriteLine("Authentication successful! Logging in...");
                return menuController.ShowUserMenu(currentUser);
            }
            else
            {
                Console.WriteLine("Invalid ID or password. Please try again.");
                Utils.PressAnyKeyToContinue();
                return false;
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
                return true;
            }
            else
            {
                Console.WriteLine("Returning to login screen...");
                return false;
            }
        }
    }
}