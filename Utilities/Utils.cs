using Assignment1_hospital_management_system.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Assignment1_hospital_management_system.Utilities
{

    // Utilities class - contains helper methods and common functionality
    public static class Utils
    {
        private static Random random = new Random();


        // Filter list of users by type
        public static List<T> FilterByType<T>(List<User> users) where T : User
        {
            return users.OfType<T>().ToList();
        }

        // Find user by ID from a list
        public static T FindUserById<T>(List<T> users, int id) where T : User
        {
            return users.FirstOrDefault(u => u.Id == id);
        }

        // Find appointments by patient ID
        public static List<Appointment> FindAppointmentsByPatientId(List<Appointment> appointments, int patientId)
        {
            return appointments.Where(a => a.PatientId == patientId).ToList();
        }

        // Find appointments by doctor ID
        public static List<Appointment> FindAppointmentsByDoctorId(List<Appointment> appointments, int doctorId)
        {
            return appointments.Where(a => a.DoctorId == doctorId).ToList();
        }

        // Find appointments between specific doctor and patient
        public static List<Appointment> FindAppointmentsByDoctorAndPatient(List<Appointment> appointments, int doctorId, int patientId)
        {
            return appointments.Where(a => a.DoctorId == doctorId && a.PatientId == patientId).ToList();
        }

        // Validate email format
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Validate phone number (basic validation)
        public static bool IsValidPhone(string phone)
        {
            return !string.IsNullOrWhiteSpace(phone) &&
                   phone.Length >= 10 &&
                   phone.All(c => char.IsDigit(c) || c == '-' || c == ' ' || c == '(' || c == ')');
        }

        // Clear console and display header
        public static void DisplayHeader(string title)
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine($"  DOTNET Hospital Management System");
            Console.WriteLine("========================================");
            Console.WriteLine($"           {title}");
            Console.WriteLine("========================================");
        }

        // Wait for user to press any key to continue
        public static void PressAnyKeyToContinue()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to return to menu...");
            Console.ReadKey();
        }

        // Get valid integer input from user
        public static int GetIntegerInput(string prompt)
        {
            int result;
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out result))
                {
                    return result;
                }
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }

        // Get non-empty string input from user
        public static string GetStringInput(string prompt)
        {
            string input;
            while (true)
            {
                Console.Write(prompt);
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input.Trim();
                }
                Console.WriteLine("Input cannot be empty. Please try again.");
            }
        }

        // Get masked password input (displays * instead of characters)
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
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
            }
            while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }


        /// <summary>
        /// Get valid ID input from user with better error handling
        /// </summary>
        /// <param name="prompt">The prompt message to display</param>
        /// <returns>Valid integer ID</returns>
        public static int GetIdInput(string prompt)
        {
            int result;
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("IDを入力してください。");
                    continue;
                }

                if (int.TryParse(input, out result))
                {
                    if (result > 0)
                    {
                        return result;
                    }
                    else
                    {
                        Console.WriteLine("正の整数を入力してください。");
                    }
                }
                else
                {
                    Console.WriteLine("有効な数値を入力してください。");
                }
            }
        }

        /// <summary>
        /// Display login instructions in Japanese
        /// </summary>
        public static void DisplayLoginInstructions()
        {
            Console.WriteLine();
            Console.WriteLine("ログイン方法:");
            Console.WriteLine("1. ユーザーIDを入力してください");
            Console.WriteLine("2. 対応するパスワードを入力してください");
            Console.WriteLine();
        }
        /// <summary>
        /// 終了オプション付きパスワード入力
        /// </summary>
        public static string GetPasswordInputWithExit(string prompt)
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
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
            }
            while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }


    }
}
