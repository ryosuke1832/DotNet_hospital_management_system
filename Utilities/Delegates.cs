using Assignment1_hospital_management_system.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment1_hospital_management_system.Utilities
{
    /// <summary>
    /// Delegate definitions and anonymous method sample implementation
    /// Assignment requirement: Examples of delegates and anonymous methods
    /// </summary>
    public static class Delegates
    {
        // Delegate definition examples
        public delegate bool UserFilter<T>(T user) where T : User;
        public delegate void LogAction(string message);
        public delegate string UserFormatter<T>(T user) where T : User;

        /// <summary>
        /// User filtering example using anonymous methods
        /// </summary>
        public static List<T> FilterUsers<T>(List<T> users, UserFilter<T> filter) where T : User
        {
            return users.Where(user => filter(user)).ToList();
        }

        /// <summary>
        /// Using delegates for logging functionality
        /// </summary>
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

        /// <summary>
        /// Example of using delegates for user display formatting
        /// </summary>
        public static void DisplayUsers<T>(List<T> users, UserFormatter<T> formatter) where T : User
        {
            foreach (T user in users)
            {
                Console.WriteLine(formatter(user));
            }
        }
    }
}