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
    /// Manages all data operations and storage
    /// </summary>
    public class DataManager
    {
        public List<Patient> Patients { get; private set; }
        public List<Doctor> Doctors { get; private set; }
        public List<Administrator> Administrators { get; private set; }
        public List<Appointment> Appointments { get; private set; }

        public DataManager()
        {
            Patients = new List<Patient>();
            Doctors = new List<Doctor>();
            Administrators = new List<Administrator>();
            Appointments = new List<Appointment>();
        }

        /// <summary>
        /// Initialize data files and load existing data
        /// </summary>
        public void Initialize()
        {
            try
            {
                Console.WriteLine("システム初期化中...");
                FileManager.InitializeDataFiles();

                Console.WriteLine("データファイルからデータを読み込み中...");
                LoadAllData();

                Console.WriteLine($"読み込み完了: 患者{Patients.Count}名, 医師{Doctors.Count}名, 管理者{Administrators.Count}名, 予約{Appointments.Count}件");

                // Always try to create sample data for debugging purposes
                ForceCreateSampleData();

                Console.WriteLine("システム初期化完了！");
                System.Threading.Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"初期化エラー: {ex.Message}");
                Console.WriteLine("サンプルデータを強制作成します...");
                ForceCreateSampleData();
            }
        }

        /// <summary>
        /// Load all data from files
        /// </summary>
        public void LoadAllData()
        {
            try
            {
                Patients = FileManager.LoadPatients() ?? new List<Patient>();
                Doctors = FileManager.LoadDoctors() ?? new List<Doctor>();
                Administrators = FileManager.LoadAdministrators() ?? new List<Administrator>();
                Appointments = FileManager.LoadAppointments() ?? new List<Appointment>();

                Console.WriteLine($"データ読み込み完了: 患者{Patients.Count}名, 医師{Doctors.Count}名, 管理者{Administrators.Count}名, 予約{Appointments.Count}件");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"データ読み込みエラー: {ex.Message}");
                // Initialize empty lists if loading fails
                Patients = new List<Patient>();
                Doctors = new List<Doctor>();
                Administrators = new List<Administrator>();
                Appointments = new List<Appointment>();
            }
        }

        /// <summary>
        /// Force create sample data for testing (always creates new data)
        /// </summary>
        private void ForceCreateSampleData()
        {
            Console.WriteLine("テスト用サンプルデータを作成します...");

            // Clear existing data for fresh start
            Patients.Clear();
            Doctors.Clear();
            Administrators.Clear();
            Appointments.Clear();

            try
            {
                // Create sample administrator with fixed ID for testing
                Administrator sampleAdmin = new Administrator("David", "Adminson")
                {
                    Id = 99999, // Fixed ID for easy testing
                    Email = "admin@hospital.com",
                    Phone = "0412345678",
                    Address = "123 Admin Street, Sydney, NSW",
                    Password = "admin123",
                    Department = "Administration"
                };
                Administrators.Add(sampleAdmin);
                Console.WriteLine($"管理者を作成しました - ID: {sampleAdmin.Id}");

                // Create sample doctor with fixed ID for testing
                Doctor sampleDoctor = new Doctor("Jack", "Doctorson", "General Practice")
                {
                    Id = 88888, // Fixed ID for easy testing
                    Email = "jack@hospital.com",
                    Phone = "0412333676",
                    Address = "23 Real Street, Sydney, NSW",
                    Password = "doctor123"
                };
                Doctors.Add(sampleDoctor);
                Console.WriteLine($"医師を作成しました - ID: {sampleDoctor.Id}");

                // Create sample patient with fixed ID for testing
                Patient samplePatient = new Patient("David", "Patientson")
                {
                    Id = 77777, // Fixed ID for easy testing
                    Email = "davey67@gmail.com",
                    Phone = "0412456876",
                    Address = "19 Real Street, Sydney, NSW",
                    Password = "patient123",
                    MedicalHistory = "No significant medical history",
                    AssignedDoctorId = sampleDoctor.Id
                };
                Patients.Add(samplePatient);
                Console.WriteLine($"患者を作成しました - ID: {samplePatient.Id}");

                // Assign patient to doctor
                sampleDoctor.AddPatient(samplePatient.Id);

                // Create sample appointment
                Appointment sampleAppointment = new Appointment(sampleDoctor.Id, samplePatient.Id, "Regular checkup with doctor");
                Appointments.Add(sampleAppointment);
                Console.WriteLine($"予約を作成しました - ID: {sampleAppointment.AppointmentId}");

                // Save sample data
                Console.WriteLine("サンプルデータをファイルに保存中...");
                SaveAllData();

                // Display created data
                Console.Clear();
                Utils.DisplayHeader("サンプルデータ作成完了");
                Console.WriteLine("テスト用のサンプルデータが正常に作成されました！");
                Console.WriteLine();
                Console.WriteLine("==========================================");
                Console.WriteLine("           テスト用ログイン情報");
                Console.WriteLine("==========================================");
                Console.WriteLine();
                Console.WriteLine("【管理者 / Administrator】");
                Console.WriteLine($"  名前: {sampleAdmin.FirstName} {sampleAdmin.LastName}");
                Console.WriteLine($"  ID: {sampleAdmin.Id}");
                Console.WriteLine($"  パスワード: admin123");
                Console.WriteLine();
                Console.WriteLine("【医師 / Doctor】");
                Console.WriteLine($"  名前: Dr. {sampleDoctor.FirstName} {sampleDoctor.LastName}");
                Console.WriteLine($"  ID: {sampleDoctor.Id}");
                Console.WriteLine($"  パスワード: doctor123");
                Console.WriteLine();
                Console.WriteLine("【患者 / Patient】");
                Console.WriteLine($"  名前: {samplePatient.FirstName} {samplePatient.LastName}");
                Console.WriteLine($"  ID: {samplePatient.Id}");
                Console.WriteLine($"  パスワード: patient123");
                Console.WriteLine();
                Console.WriteLine("==========================================");
                Console.WriteLine("上記のIDとパスワードを必ずメモしてください！");
                Console.WriteLine("==========================================");
                Utils.PressAnyKeyToContinue();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"サンプルデータ作成エラー: {ex.Message}");
                Console.WriteLine("手動でユーザーを作成してください。");
            }
        }

        /// <summary>
        /// Save all data to files
        /// </summary>
        public void SaveAllData()
        {
            try
            {
                FileManager.SaveAllData(Patients, Doctors, Administrators, Appointments);
                Console.WriteLine("データ保存完了！");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"データ保存エラー: {ex.Message}");
            }
        }

        /// <summary>
        /// Find user by ID and password across all user types
        /// </summary>
        public User FindUser(int id, string password)
        {
            // Debug output
            Console.WriteLine($"ユーザー検索中: ID={id}, パスワード={password}");
            Console.WriteLine($"検索対象: 患者{Patients.Count}名, 医師{Doctors.Count}名, 管理者{Administrators.Count}名");

            // Check patients
            User user = Patients.FirstOrDefault(p => p.Id == id && p.Password == password);
            if (user != null)
            {
                Console.WriteLine($"患者として認証成功: {user.FirstName} {user.LastName}");
                return user;
            }

            // Check doctors
            user = Doctors.FirstOrDefault(d => d.Id == id && d.Password == password);
            if (user != null)
            {
                Console.WriteLine($"医師として認証成功: {user.FirstName} {user.LastName}");
                return user;
            }

            // Check administrators
            user = Administrators.FirstOrDefault(a => a.Id == id && a.Password == password);
            if (user != null)
            {
                Console.WriteLine($"管理者として認証成功: {user.FirstName} {user.LastName}");
                return user;
            }

            Console.WriteLine("認証失敗: 該当するユーザーが見つかりません");
            return null;
        }

        /// <summary>
        /// Add a new patient to the system
        /// </summary>
        public void AddPatient(Patient patient)
        {
            Patients.Add(patient);
            SaveAllData();
        }

        /// <summary>
        /// Add a new doctor to the system
        /// </summary>
        public void AddDoctor(Doctor doctor)
        {
            Doctors.Add(doctor);
            SaveAllData();
        }

        /// <summary>
        /// Add a new appointment to the system
        /// </summary>
        public void AddAppointment(Appointment appointment)
        {
            Appointments.Add(appointment);
            SaveAllData();
        }
    }
}