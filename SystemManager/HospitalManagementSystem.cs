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


            User.SetDataManager(dataManager);
            Appointment.SetDataManager(dataManager);

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

                // Display current user count for debugging
                DisplaySystemStatus();

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
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Detail: {ex.StackTrace}");
                Utils.PressAnyKeyToContinue();
            }
        }

        private void InitializeSystem()
        {
            Console.WriteLine("DOTNET Hospital Management System Initializing");
            dataManager.Initialize();
            dataManager.RegisterExistingIds(); 


            System.Threading.Thread.Sleep(1000);
        }

        /// <summary>
        /// Display current system status for debugging
        /// </summary>
        private void DisplaySystemStatus()
        {
            Console.WriteLine();
            Console.WriteLine("=== システム状態 ===");
            Console.WriteLine($"登録済み管理者: {dataManager.Administrators.Count}名");
            Console.WriteLine($"登録済み医師: {dataManager.Doctors.Count}名");
            Console.WriteLine($"登録済み患者: {dataManager.Patients.Count}名");
            Console.WriteLine($"登録済み予約: {dataManager.Appointments.Count}件");

            // Display actual IDs if sample data exists
            if (dataManager.Administrators.Count > 0 || dataManager.Doctors.Count > 0 || dataManager.Patients.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("=== 利用可能なテストアカウント ===");

                foreach (var admin in dataManager.Administrators.Where(a => a.Password == "admin123"))
                {
                    Console.WriteLine($"管理者: {admin.FirstName} {admin.LastName} (ID: {admin.Id})");
                }

                foreach (var doctor in dataManager.Doctors.Where(d => d.Password == "doctor123"))
                {
                    Console.WriteLine($"医師: Dr. {doctor.FirstName} {doctor.LastName} (ID: {doctor.Id})");
                }

                foreach (var patient in dataManager.Patients.Where(p => p.Password == "patient123"))
                {
                    Console.WriteLine($"患者: {patient.FirstName} {patient.LastName} (ID: {patient.Id})");
                }
            }
            Console.WriteLine("==================");
            System.Threading.Thread.Sleep(2000);
        }

        private void Shutdown()
        {
            dataManager.SaveAllData();
            Console.WriteLine("システム終了前のクリーンアップを実行中...");
            dataManager.ShowMemoryUsageAndCleanup();
            Console.WriteLine("DOTNET Hospital Management System をご利用いただき、ありがとうございました！");
            Console.WriteLine("何かキーを押して終了してください...");
            Console.ReadKey();
        }
    }
}