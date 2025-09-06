using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Models/Receptionist.cs - 新しいファイルを作成

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1_hospital_management_system.Models
{
    /// <summary>
    /// 受付嬢クラス - 病院の受付業務を担当
    /// 機能：新規患者登録、既存患者閲覧、予約一覧表示、予約追加
    /// </summary>
    public class Receptionist : User
    {
        public string Department { get; set; }
        public string Shift { get; set; }

        // コンストラクタ
        public Receptionist() : base()
        {
            Department = "Reception";
            Shift = "Morning";
        }

        public Receptionist(string firstName, string lastName) : base(firstName, lastName)
        {
            Department = "Reception";
            Shift = "Morning";
        }

        // 抽象メソッドのオーバーライド - 受付嬢メニュー
        public override void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("         Receptionist Menu");
            Console.WriteLine("========================================");
            Console.WriteLine("Welcome to DOTNET Hospital Management System");
            Console.WriteLine();
            Console.WriteLine("Please choose an option:");
            Console.WriteLine("1. Register new patient");        // 新規患者登録
            Console.WriteLine("2. View existing patients");      // 既存患者閲覧
            Console.WriteLine("3. View appointments");           // 予約一覧表示
            Console.WriteLine("4. Add new appointment");         // 予約追加
            Console.WriteLine("5. Logout");
            Console.WriteLine("6. Exit");
            Console.WriteLine("========================================");
        }

        public override string GetUserType()
        {
            return "Receptionist";
        }

        // メソッドオーバーライド - 受付嬢固有の表示
        public override string ToString()
        {
            return $"Receptionist: {FirstName} {LastName} (ID: {Id}) | Department: {Department}";
        }

        // 受付嬢固有のメソッド - メソッドオーバーロードの例

        /// <summary>
        /// 新規患者の登録（メソッドオーバーロード例）
        /// </summary>
        public Patient RegisterNewPatient(string firstName, string lastName, string phone)
        {
            Console.WriteLine($"新規患者を登録: {firstName} {lastName}");
            Console.WriteLine($"電話番号: {phone}");

            Patient newPatient = new Patient(firstName, lastName)
            {
                Phone = phone,
                Email = $"{firstName.ToLower()}.{lastName.ToLower()}@email.com",
                Address = "住所未入力",
                Password = "patient123"
            };

            Console.WriteLine($"患者ID {newPatient.Id} で登録完了");
            return newPatient;
        }

        /// <summary>
        /// 新規患者の登録（全情報版 - メソッドオーバーロード例）
        /// </summary>
        public Patient RegisterNewPatient(string firstName, string lastName, string phone, string email, string address)
        {
            Console.WriteLine($"新規患者を登録: {firstName} {lastName}");

            Patient newPatient = new Patient(firstName, lastName)
            {
                Phone = phone,
                Email = email,
                Address = address,
                Password = "patient123"
            };

            Console.WriteLine($"患者ID {newPatient.Id} で登録完了");
            return newPatient;
        }

        /// <summary>
        /// 予約の作成（メソッドオーバーロード例）
        /// </summary>
        public Appointment CreateAppointment(int patientId, int doctorId, string description)
        {
            Console.WriteLine($"予約を作成中...");
            Console.WriteLine($"患者ID: {patientId} | 医師ID: {doctorId}");

            Appointment appointment = new Appointment(doctorId, patientId, description);
            Console.WriteLine($"予約ID {appointment.AppointmentId} で予約作成完了");
            return appointment;
        }

        /// <summary>
        /// 予約の作成（Patientオブジェクト版 - メソッドオーバーロード例）
        /// </summary>
        public Appointment CreateAppointment(Patient patient, Doctor doctor, string description)
        {
            return CreateAppointment(patient.Id, doctor.Id, description);
        }
    }
}