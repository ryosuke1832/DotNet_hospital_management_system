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
            Utils.DisplayHeader("ログイン");

            Console.WriteLine();
            Console.WriteLine("ログイン方法:");
            Console.WriteLine("1. 下記のテストユーザーIDを入力してください");
            Console.WriteLine("2. 対応するパスワードを入力してください");
            Console.WriteLine("3. パスワードは画面上で'*'として表示されます");
            Console.WriteLine("4. システムを終了する場合は 'exit' と入力してください");
            Console.WriteLine();

            Console.WriteLine();
            Console.WriteLine("=== ログイン ===");

            // ユーザーID入力（終了チェック付き）
            string userIdInput = Utils.GetStringInput("ユーザーID (終了する場合は 'exit'): ");

            // 終了コマンドチェック
            if (IsExitCommand(userIdInput))
            {
                return ConfirmExit();
            }

            // IDの数値変換チェック
            if (!int.TryParse(userIdInput, out int userId))
            {
                Console.WriteLine("無効なユーザーIDです。数値を入力してください。");
                Utils.PressAnyKeyToContinue();
                return false; // ログイン画面に戻る
            }

            // パスワード入力（終了チェック付き）
            string password = Utils.GetPasswordInputWithExit("パスワード (終了する場合は 'exit'): ");

            // パスワード入力での終了チェック
            if (IsExitCommand(password))
            {
                return ConfirmExit();
            }

            // 認証処理
            User currentUser = dataManager.FindUser(userId, password);

            if (currentUser != null)
            {
                Console.WriteLine("認証成功！システムにログインしています...");
                System.Threading.Thread.Sleep(1500);

                // 適切なメニューを表示
                return menuController.ShowUserMenu(currentUser);
            }
            else
            {
                Console.WriteLine("IDまたはパスワードが正しくありません。再度お試しください。");
                Utils.PressAnyKeyToContinue();
                return false; // ログイン画面に戻る
            }
        }

        /// <summary>
        /// 終了コマンドかどうかをチェック
        /// </summary>
        private bool IsExitCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string normalized = input.Trim().ToLower();
            return normalized == "exit" ||
                   normalized == "quit" ||
                   normalized == "終了" ||
                   normalized == "q";
        }

        /// <summary>
        /// 終了確認
        /// </summary>
        private bool ConfirmExit()
        {
            Console.WriteLine();
            Console.WriteLine("システムを終了しますか？");
            string confirmation = Utils.GetStringInput("終了する場合は 'y' または 'yes' を入力: ");

            string normalized = confirmation.Trim().ToLower();
            if (normalized == "y" || normalized == "yes" || normalized == "はい")
            {
                Console.WriteLine("システムを終了します...");
                System.Threading.Thread.Sleep(1000);
                return true; // システム終了
            }
            else
            {
                Console.WriteLine("ログイン画面に戻ります...");
                System.Threading.Thread.Sleep(1000);
                return false; // ログイン画面に戻る
            }
        }
    }
}