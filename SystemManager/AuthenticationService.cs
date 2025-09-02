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
            DisplayTestUserInfo();

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

        /// <summary>
        /// Always display test user information - including default admin
        /// </summary>
        private void DisplayTestUserInfo()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("          テスト用ユーザー情報");
            Console.WriteLine("==========================================");

            // Always show default admin (ID: 99999)
            Console.WriteLine("【デフォルト管理者】");
            Console.WriteLine("  名前: System Administrator");
            Console.WriteLine("  ID: 99999");
            Console.WriteLine("  パスワード: admin1234");
            Console.WriteLine("  部門: Administration");
            Console.WriteLine();

            // Check if we have actual sample data
            var doctor = dataManager.Doctors.FirstOrDefault(d => d.Password == "doctor123");
            var patient = dataManager.Patients.FirstOrDefault(p => p.Password == "patient123");

            if (doctor != null)
            {
                Console.WriteLine("【医師】");
                Console.WriteLine($"  名前: Dr. {doctor.FirstName} {doctor.LastName}");
                Console.WriteLine($"  ID: {doctor.Id}");
                Console.WriteLine($"  パスワード: doctor123");
                Console.WriteLine($"  専門: {doctor.Specialization}");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("【医師（サンプル）】");
                Console.WriteLine("  名前: Dr. Jack Doctorson");
                Console.WriteLine("  ID: (自動生成されます)");
                Console.WriteLine("  パスワード: doctor123");
                Console.WriteLine();
            }

            if (patient != null)
            {
                Console.WriteLine("【患者】");
                Console.WriteLine($"  名前: {patient.FirstName} {patient.LastName}");
                Console.WriteLine($"  ID: {patient.Id}");
                Console.WriteLine($"  パスワード: patient123");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("【患者（サンプル）】");
                Console.WriteLine("  名前: David Patientson");
                Console.WriteLine("  ID: (自動生成されます)");
                Console.WriteLine("  パスワード: patient123");
                Console.WriteLine();
            }

            Console.WriteLine("==========================================");
            Console.WriteLine("上記のIDとパスワードでログインできます");
            Console.WriteLine("注意: プログラム起動時に自動的にサンプルデータが作成されます");
            Console.WriteLine("==========================================");
        }
    }
}