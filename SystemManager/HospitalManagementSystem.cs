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
    /// Main system controller - manages the overall hospital management system
    /// </summary>
    public class HospitalSystem
    {
        private DataManager dataManager;
        private AuthenticationService authService;
        private MenuController menuController;

        public HospitalSystem()
        {
            // Initialize all system components
            dataManager = new DataManager();
            authService = new AuthenticationService(dataManager);
            menuController = new MenuController(dataManager);
        }

        /// <summary>
        /// Main entry point for the hospital system
        /// </summary>
        public void Run()
        {
            try
            {
                // Initialize the system
                InitializeSystem();

                // Main application loop
                bool exitApplication = false;
                while (!exitApplication)
                {
                    exitApplication = authService.HandleLogin(menuController);
                }

                // Clean shutdown
                Shutdown();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"System error: {ex.Message}");
                Utils.PressAnyKeyToContinue();
            }
        }

        private void InitializeSystem()
        {
            Console.WriteLine("Initializing DOTNET Hospital Management System...");
            dataManager.Initialize();
            Console.WriteLine("System initialized successfully!");

            // サンプルデータが表示された場合はユーザーが既にキーを押しているので、
            // 追加の待機は不要。サンプルデータが作成されなかった場合のみ短い待機。
            if (dataManager.Patients.Count == 0 && dataManager.Doctors.Count == 0 && dataManager.Administrators.Count == 0)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void Shutdown()
        {
            dataManager.SaveAllData();
            Console.WriteLine("Thank you for using DOTNET Hospital Management System!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}