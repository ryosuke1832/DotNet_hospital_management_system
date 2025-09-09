
using Assignment1_hospital_management_system.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment1_hospital_management_system.Utilities
{
    public static class Utils
    {

        // Generic methods 
        public static T FindById<T>(this IEnumerable<T> items, int id) where T : User =>
            items.FirstOrDefault(u => u.Id == id);

        public static List<Appointment> ForPatient(this IEnumerable<Appointment> appointments, int patientId) =>
            appointments.Where(a => a.PatientId == patientId).ToList();

        public static List<Appointment> ForDoctor(this IEnumerable<Appointment> appointments, int doctorId) =>
            appointments.Where(a => a.DoctorId == doctorId).ToList();

        public static List<Appointment> Between(this IEnumerable<Appointment> appointments, int doctorId, int patientId) =>
            appointments.Where(a => a.DoctorId == doctorId && a.PatientId == patientId).ToList();

        // Console operations
        public static void DisplayHeader(string title)
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("  DOTNET Hospital Management System");
            Console.WriteLine("========================================");
            Console.WriteLine($"           {title}");
            Console.WriteLine("========================================");
        }

        public static void PressAnyKeyToContinue()
        {
            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }

        public static int GetIntegerInput(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int result))
                    return result;
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }

        public static string GetStringInput(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(input))
                    return input;
                Console.WriteLine("Input cannot be empty. Please try again.");
            }
        }

        public static string GetPasswordInput(string prompt)
        {
            Console.Write(prompt);
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[..^1];
                    Console.Write("\b \b");
                }
            }
            while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }

        public static string GetPasswordInputWithExit(string prompt) => GetPasswordInput(prompt);
    }

    public static class Delegates
    {
        public delegate bool UserFilter<T>(T user) where T : User;
        public delegate void LogAction(string message);
        public delegate string UserFormatter<T>(T user) where T : User;

        public static List<T> FilterUsers<T>(List<T> users, UserFilter<T> filter) where T : User =>
            users.Where(user => filter(user)).ToList();

        public static void ExecuteWithLogging(string actionName, LogAction logger, Action action)
        {
            logger($"Started: {actionName}");
            try
            {
                action();
                logger($"Completed: {actionName}");
            }
            catch (Exception ex)
            {
                logger($"Error: {actionName} - {ex.Message}");
                throw;
            }
        }

        public static void DisplayUsers<T>(List<T> users, UserFormatter<T> formatter) where T : User
        {
            foreach (T user in users)
                Console.WriteLine(formatter(user));
        }
    }
}