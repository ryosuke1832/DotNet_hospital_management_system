using Assignment1_hospital_management_system.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment1_hospital_management_system.Utilities
{
    /// <summary>
    /// デリゲート定義と匿名メソッドのサンプル実装
    /// 課題要件: デリゲートと匿名メソッドの例
    /// </summary>
    public static class Delegates
    {
        // デリゲート定義例
        public delegate bool UserFilter<T>(T user) where T : User;
        public delegate void LogAction(string message);
        public delegate string UserFormatter<T>(T user) where T : User;

        /// <summary>
        /// 匿名メソッドを使用したユーザーフィルタリング例
        /// </summary>
        public static List<T> FilterUsers<T>(List<T> users, UserFilter<T> filter) where T : User
        {
            return users.Where(user => filter(user)).ToList();
        }

        /// <summary>
        /// ログ機能にデリゲートを使用
        /// </summary>
        public static void ExecuteWithLogging(string actionName, LogAction logger, Action action)
        {
            logger($"開始: {actionName}");
            try
            {
                action();
                logger($"完了: {actionName}");
            }
            catch (Exception ex)
            {
                logger($"エラー: {actionName} - {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// ユーザー表示フォーマット用デリゲート使用例
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