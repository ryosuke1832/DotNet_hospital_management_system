
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment1_hospital_management_system.Utilities
{
    public static class Extensions
    {
        /// <summary>
        /// Extension method to determine if a string is in valid email address format
        /// Usage example: "test@email.com".IsValidEmail()
        /// </summary>
        public static bool IsValidEmail(this string email)
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

        /// <summary>
        /// Extension method to determine if a phone number is valid
        /// Usage example: "0412345678".IsValidPhone()
        /// </summary>
        public static bool IsValidPhone(this string phone)
        {
            return !string.IsNullOrWhiteSpace(phone) && phone.Length >= 10 &&
                   phone.All(c => char.IsDigit(c) || c == '-' || c == ' ' || c == '(' || c == ')');
        }

        /// <summary>
        /// Extension method to safely get substring with bounds checking
        /// Usage example: "Hello World".SafeSubstring(0, 5)
        /// </summary>
        public static string SafeSubstring(this string str, int startIndex, int length)
        {
            if (string.IsNullOrEmpty(str) || startIndex >= str.Length)
                return string.Empty;

            if (startIndex + length > str.Length)
                length = str.Length - startIndex;

            return str.Substring(startIndex, length);
        }

        /// <summary>
        /// Extension method to capitalize first letter of each word
        /// Usage example: "john smith".ToTitleCase() -> "John Smith"
        /// </summary>
        public static string ToTitleCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }
    }
}