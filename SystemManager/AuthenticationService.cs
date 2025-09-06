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
            Utils.DisplayHeader("ログイン");

            // Always display test user information

            Console.WriteLine();
            Console.WriteLine("ログイン方法:");
            Console.WriteLine("1. 上記のテストユーザーIDを入力してください");
            Console.WriteLine("2. 対応するパスワードを入力してください");
            Console.WriteLine("3. パスワードは画面上で'*'として表示されます");
            Console.WriteLine();

            int userId = Utils.GetIntegerInput("ユーザーID: ");
            string password = Utils.GetPasswordInput("パスワード: ");

            User currentUser = dataManager.FindUser(userId, password);

            if (currentUser != null)
            {
                Console.WriteLine("認証成功！システムにログインしています...");
                System.Threading.Thread.Sleep(1500);

                // Show appropriate menu based on user type
                return menuController.ShowUserMenu(currentUser);
            }
            else
            {
                Console.WriteLine("IDまたはパスワードが正しくありません。再度お試しください。");
                Utils.PressAnyKeyToContinue();
                return false;
            }
        }
    }

}