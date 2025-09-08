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
            Console.WriteLine("DOTNET Hospital Management System Initializing...");
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
            Console.WriteLine("=== System Status ===");
            Console.WriteLine($"Registered Administrators: {dataManager.Administrators.Count}");
            Console.WriteLine($"Registered Doctors: {dataManager.Doctors.Count}");
            Console.WriteLine($"Registered Patients: {dataManager.Patients.Count}");
            Console.WriteLine($"Registered Appointments: {dataManager.Appointments.Count}");

            // Display actual IDs if sample data exists
            if (dataManager.Administrators.Count > 0 || dataManager.Doctors.Count > 0 || dataManager.Patients.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("=== Available Test Accounts ===");

                foreach (var admin in dataManager.Administrators.Where(a => a.Password == "admin123"))
                {
                    Console.WriteLine($"Administrator: {admin.FirstName} {admin.LastName} (ID: {admin.Id})");
                }

                foreach (var doctor in dataManager.Doctors.Where(d => d.Password == "doctor123"))
                {
                    Console.WriteLine($"Doctor: Dr. {doctor.FirstName} {doctor.LastName} (ID: {doctor.Id})");
                }

                foreach (var patient in dataManager.Patients.Where(p => p.Password == "patient123"))
                {
                    Console.WriteLine($"Patient: {patient.FirstName} {patient.LastName} (ID: {patient.Id})");
                }

                foreach (var receptionist in dataManager.Receptionists.Where(r => r.Password == "reception123"))
                {
                    Console.WriteLine($"Receptionist: {receptionist.FirstName} {receptionist.LastName} (ID: {receptionist.Id})");
                }
            }
            Console.WriteLine("======================");
            System.Threading.Thread.Sleep(2000);
        }

        private void Shutdown()
        {
            dataManager.SaveAllData();
            Console.WriteLine("Performing cleanup before system shutdown...");
            dataManager.ShowMemoryUsageAndCleanup();
            Console.WriteLine("Thank you for using the DOTNET Hospital Management System!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}