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
        /// 文字列がメールアドレス形式かどうかを判定する拡張メソッド
        /// 使用例: "test@email.com".IsValidEmail()
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
