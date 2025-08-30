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
        /// Always display test user information - even if no sample data exists yet
        /// </summary>
        private void DisplayTestUserInfo()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("          テスト用ユーザー情報");
            Console.WriteLine("==========================================");

            // Check if we have actual sample data
            var admin = dataManager.Administrators.FirstOrDefault(a => a.Password == "admin123");
            var doctor = dataManager.Doctors.FirstOrDefault(d => d.Password == "doctor123");
            var patient = dataManager.Patients.FirstOrDefault(p => p.Password == "patient123");

            if (admin != null || doctor != null || patient != null)
            {
                // Display actual sample data
                if (admin != null)
                {
                    Console.WriteLine("【管理者】");
                    Console.WriteLine($"  名前: {admin.FirstName} {admin.LastName}");
                    Console.WriteLine($"  ID: {admin.Id}");
                    Console.WriteLine($"  パスワード: admin123");
                    Console.WriteLine();
                }

                if (doctor != null)
                {
                    Console.WriteLine("【医師】");
                    Console.WriteLine($"  名前: Dr. {doctor.FirstName} {doctor.LastName}");
                    Console.WriteLine($"  ID: {doctor.Id}");
                    Console.WriteLine($"  パスワード: doctor123");
                    Console.WriteLine($"  専門: {doctor.Specialization}");
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
            }
            else
            {
                // Display fixed test IDs if no sample data exists yet
                Console.WriteLine("固定テストIDを使用してください：");
                Console.WriteLine();
                Console.WriteLine("【管理者】");
                Console.WriteLine("  名前: David Adminson");
                Console.WriteLine("  ID: 99999");
                Console.WriteLine("  パスワード: admin123");
                Console.WriteLine();
                Console.WriteLine("【医師】");
                Console.WriteLine("  名前: Dr. Jack Doctorson");
                Console.WriteLine("  ID: 88888");
                Console.WriteLine("  パスワード: doctor123");
                Console.WriteLine();
                Console.WriteLine("【患者】");
                Console.WriteLine("  名前: David Patientson");
                Console.WriteLine("  ID: 77777");
                Console.WriteLine("  パスワード: patient123");
                Console.WriteLine();
                Console.WriteLine("注意: プログラム起動時に自動的にこれらのIDでサンプルデータが作成されます");
            }

            Console.WriteLine("==========================================");
            Console.WriteLine("上記のIDとパスワードでログインできます");
            Console.WriteLine("==========================================");
        }
    }
}