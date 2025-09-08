using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}