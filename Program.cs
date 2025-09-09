using System;
using System.Collections.Generic;
using System.Linq;
using System;
using Assignment1_hospital_management_system.SystemManager;

namespace Assignment1_hospital_management_system

{
    /// Main program entry point - creates and runs the hospital management system
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Create and run the hospital management system
                HospitalSystem hospitalSystem = new HospitalSystem();
                hospitalSystem.Run();
            }
            catch (Exception ex)
            {
                // Handle any critical system errors
                Console.WriteLine("Critical system error occurred:");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Please restart the application.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}