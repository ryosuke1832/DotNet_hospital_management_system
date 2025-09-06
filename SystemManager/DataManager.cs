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
    /// Manages all data operations and storage with improved file handling
    /// </summary>
    public class DataManager
    {
        private const string DEFAULT_ADMIN_PASSWORD = "admin1234";
        private const string DEFAULT_ADMIN_EMAIL = "admin@hospital.com";


        private HashSet<int> usedIds = new HashSet<int>();
        private Random random = new Random();

        public List<Patient> Patients { get; private set; }
        public List<Doctor> Doctors { get; private set; }
        public List<Administrator> Administrators { get; private set; }
        public List<Appointment> Appointments { get; private set; }

        public List<Receptionist> Receptionists { get; private set; }

        /// <summary>
        /// Constructor - initializes empty collections
        /// </summary>
        public DataManager()
        {
            Patients = new List<Patient>();
            Doctors = new List<Doctor>();
            Administrators = new List<Administrator>();
            Appointments = new List<Appointment>();
            Receptionists = new List<Receptionist>();
        }

        /// <summary>
        /// Initialize data files and load existing data
        /// </summary>
        public void Initialize()
        {
            Console.WriteLine("=== Initializing Data Manager ===");

            // Initialize file system
            FileManager.InitializeDataFiles();

            // Display file information for debugging
            FileManager.DisplayFileInformation();

            // Load existing data
            LoadAllData();

            CreateDefaultAdminIfNeeded();


            Console.WriteLine("=== Data Manager initialized successfully ===");
        }

        /// <summary>
        /// デフォルトAdministratorを作成（Adminが存在しない場合のみ）
        /// </summary>
        private void CreateDefaultAdminIfNeeded()
        {
            // Administratorが1人も存在しない場合のみ作成
            if (Administrators.Count == 0)
            {
                Console.WriteLine("No administrators found. Creating default administrator...");

                // 通常のコンストラクタを使用（自動ID生成）
                Administrator defaultAdmin = new Administrator("System", "Administrator")
                {
                    Email = DEFAULT_ADMIN_EMAIL,
                    Phone = "0412345678",
                    Address = "123 Admin Street, Sydney, NSW",
                    Password = DEFAULT_ADMIN_PASSWORD,
                    Department = "System Administration",
                    AccessLevel = "Full Access"
                };

                // システムに追加
                Administrators.Add(defaultAdmin);

                // ファイルに即座に保存
                SaveAllData();

                Console.WriteLine("=== Default Administrator Created Successfully ===");
                Console.WriteLine($"Name: {defaultAdmin.FirstName} {defaultAdmin.LastName}");
                Console.WriteLine($"ID: {defaultAdmin.Id}"); // 自動生成されたID
                Console.WriteLine($"Email: {defaultAdmin.Email}");
                Console.WriteLine($"Password: {DEFAULT_ADMIN_PASSWORD}");
                Console.WriteLine($"Department: {defaultAdmin.Department}");
                Console.WriteLine("================================================");
            }
            else
            {
                Console.WriteLine($"Administrators already exist ({Administrators.Count} found). Skipping default admin creation.");

                // 既存のAdmin情報を表示（デバッグ用）
                foreach (var admin in Administrators)
                {
                    Console.WriteLine($"Existing Admin: {admin.FirstName} {admin.LastName} (ID: {admin.Id})");
                }
            }
        }




        /// <summary>
        /// Load all data from files with comprehensive logging
        /// </summary>
        public void LoadAllData()
        {
            try
            {
                Console.WriteLine("Loading data from files...");

                Patients = FileManager.LoadPatients();
                Doctors = FileManager.LoadDoctors();
                Administrators = FileManager.LoadAdministrators();
                Appointments = FileManager.LoadAppointments();
                Receptionists = FileManager.LoadReceptionists();

                Console.WriteLine($"Data loading summary:");
                Console.WriteLine($"- Patients: {Patients.Count}");
                Console.WriteLine($"- Doctors: {Doctors.Count}");
                Console.WriteLine($"- Administrators: {Administrators.Count}");
                Console.WriteLine($"- Appointments: {Appointments.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        /// <summary>
        /// ユーザーとアポイントメント両方で使用できる統一ID生成メソッド
        /// </summary>
        public int GenerateUniqueId()
        {
            int newId;
            int maxAttempts = 1000;
            int attempts = 0;

            do
            {
                newId = random.Next(10000, 99999);
                attempts++;

                if (attempts >= maxAttempts)
                {
                    throw new InvalidOperationException("Failed to generate unique ID. System may be at capacity.");
                }
            }
            while (usedIds.Contains(newId));

            usedIds.Add(newId);
            Console.WriteLine($"Generated unique ID: {newId}");
            return newId;
        }

        /// <summary>
        /// システム初期化時に既存IDを登録
        /// </summary>
        public void RegisterExistingIds()
        {
            // 既存のユーザーIDを登録
            foreach (var patient in Patients)
                usedIds.Add(patient.Id);
            foreach (var doctor in Doctors)
                usedIds.Add(doctor.Id);
            foreach (var admin in Administrators)
                usedIds.Add(admin.Id);
            foreach (var receptionist in Receptionists)
                usedIds.Add(receptionist.Id);

            // 既存のアポイントメントIDを登録
            foreach (var appointment in Appointments)
                usedIds.Add(appointment.AppointmentId);
        }

        /// <summary>
        /// システムのメモリ使用量を表示し、ガベージコレクションを実行するメソッド
        /// 課題要件「ガベージコレクションの例」を満たすためのメソッド
        /// </summary>
        public void ShowMemoryUsageAndCleanup()
        {
            try
            {
                Console.WriteLine("=== メモリ使用状況とガベージコレクション ===");

                // ガベージコレクション実行前のメモリ使用量
                long memoryBefore = GC.GetTotalMemory(false);
                Console.WriteLine($"ガベージコレクション前のメモリ使用量: {memoryBefore / 1024:N0} KB");
                Console.WriteLine($"Generation 0 collections: {GC.CollectionCount(0)}");
                Console.WriteLine($"Generation 1 collections: {GC.CollectionCount(1)}");
                Console.WriteLine($"Generation 2 collections: {GC.CollectionCount(2)}");

                // 明示的にガベージコレクションを実行
                Console.WriteLine("ガベージコレクションを実行中...");
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                // ガベージコレクション実行後のメモリ使用量
                long memoryAfter = GC.GetTotalMemory(true);
                Console.WriteLine($"ガベージコレクション後のメモリ使用量: {memoryAfter / 1024:N0} KB");

                long memoryFreed = memoryBefore - memoryAfter;
                if (memoryFreed > 0)
                {
                    Console.WriteLine($"解放されたメモリ: {memoryFreed / 1024:N0} KB");
                }
                else
                {
                    Console.WriteLine("現在、解放可能な未使用メモリはありません");
                }

                Console.WriteLine("=== メモリクリーンアップ完了 ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"メモリ管理中にエラーが発生しました: {ex.Message}");
            }
        }



        /// <summary>
        /// Generate unique user ID that doesn't conflict with existing users
        /// </summary>
        /// <returns>Unique ID</returns>
        public int GenerateUniqueUserId()
        {
            Random random = new Random();
            int newId;
            int maxAttempts = 1000; // Prevent infinite loop
            int attempts = 0;

            do
            {
                // Generate ID in range 10000-99999 for all user types
                newId = random.Next(10000, 99999);

                attempts++;
                if (attempts >= maxAttempts)
                {
                    throw new InvalidOperationException("Failed to generate unique ID. System may be at capacity.");
                }
            }
            while (IsIdAlreadyExists(newId));

            Console.WriteLine($"Generated unique ID: {newId}");
            return newId;
        }

        /// <summary>
        /// Check if the specified ID already exists in the system
        /// </summary>
        /// <param name="id">ID to check</param>
        /// <returns>True if ID exists, false otherwise</returns>
        private bool IsIdAlreadyExists(int id)
        {
            // Check all user types for ID conflicts
            return Patients.Any(p => p.Id == id) ||
                   Doctors.Any(d => d.Id == id) ||
                   Administrators.Any(a => a.Id == id);
        }


        /// <summary>
        /// Save all data to files with comprehensive logging
        /// </summary>
        public void SaveAllData()
        {
            try
            {
                Console.WriteLine("Saving all data to files...");
                FileManager.SaveAllData(Patients, Doctors, Administrators, Appointments,Receptionists);
                Console.WriteLine("Data save operation completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }



        /// <summary>
        /// 受付嬢をシステムに追加
        /// </summary>
        public void AddReceptionist(Receptionist receptionist)
        {
            if (receptionist != null)
            {
                Receptionists.Add(receptionist);
                Console.WriteLine($"Receptionist {receptionist.FirstName} {receptionist.LastName} (ID: {receptionist.Id}) added to system");
                SaveAllData();
            }
            else
            {
                Console.WriteLine("Error: Cannot add null receptionist");
            }
        }


        /// <summary>
        /// Display sample data information for testing purposes
        /// </summary>
        private void DisplaySampleDataInformation(Doctor doctor, Patient patient)
        {
            Console.Clear();
            Utils.DisplayHeader("Sample Test Data Created");
            Console.WriteLine("The following test users have been created for demonstration:");
            Console.WriteLine();

            // Always show default admin
            var defaultAdmin = Administrators.FirstOrDefault(a => a.Id == 99999);
            if (defaultAdmin != null)
            {
                Console.WriteLine("=== DEFAULT ADMINISTRATOR ===");
                Console.WriteLine($"Name: {defaultAdmin.FirstName} {defaultAdmin.LastName}");
                Console.WriteLine($"ID: {defaultAdmin.Id}");
                Console.WriteLine($"Password: admin1234");
                Console.WriteLine($"Department: {defaultAdmin.Department}");
                Console.WriteLine();
            }

            if (doctor != null)
            {
                Console.WriteLine("=== DOCTOR ===");
                Console.WriteLine($"Name: Dr. {doctor.FirstName} {doctor.LastName}");
                Console.WriteLine($"ID: {doctor.Id}");
                Console.WriteLine($"Password: doctor123");
                Console.WriteLine($"Specialization: {doctor.Specialization}");
                Console.WriteLine();
            }

            if (patient != null)
            {
                Console.WriteLine("=== PATIENT ===");
                Console.WriteLine($"Name: {patient.FirstName} {patient.LastName}");
                Console.WriteLine($"ID: {patient.Id}");
                Console.WriteLine($"Password: patient123");
                if (patient.AssignedDoctorId.HasValue && doctor != null)
                {
                    Console.WriteLine($"Assigned Doctor: Dr. {doctor.FirstName} {doctor.LastName}");
                }
                Console.WriteLine();
            }

            Console.WriteLine("========================================");
            Console.WriteLine("You can use any of the above credentials to test the system.");
            Console.WriteLine("Please write down or screenshot this information for testing.");
            Console.WriteLine("All data has been saved to the Data folder.");
            Console.WriteLine("========================================");
            Utils.PressAnyKeyToContinue();
        }

        /// <summary>
        /// Find user by ID and password across all user types
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="password">User password</param>
        /// <returns>User object if found, null otherwise</returns>
        public User FindUser(int id, string password)
        {
            Console.WriteLine($"Searching for user with ID: {id}");
            Console.WriteLine($"Available users in memory:");
            Console.WriteLine($"- Patients: {Patients.Count}");
            Console.WriteLine($"- Doctors: {Doctors.Count}");
            Console.WriteLine($"- Administrators: {Administrators.Count}");

            // Debug: Show all admin IDs and passwords
            if (Administrators.Count > 0)
            {
                Console.WriteLine("Administrator IDs in memory:");
                foreach (var admin in Administrators)
                {
                    Console.WriteLine($"  ID: {admin.Id}, Password: {admin.Password}, Name: {admin.FirstName} {admin.LastName}");
                }
            }

            // Check patients
            User user = Patients.FirstOrDefault(p => p.Id == id && p.Password == password);
            if (user != null)
            {
                Console.WriteLine($"Patient login successful: {user.FirstName} {user.LastName}");
                return user;
            }

            // Check doctors
            user = Doctors.FirstOrDefault(d => d.Id == id && d.Password == password);
            if (user != null)
            {
                Console.WriteLine($"Doctor login successful: Dr. {user.FirstName} {user.LastName}");
                return user;
            }

            // Check administrators
            user = Administrators.FirstOrDefault(a => a.Id == id && a.Password == password);
            if (user != null)
            {
                Console.WriteLine($"Administrator login successful: {user.FirstName} {user.LastName}");
                return user;
            }

            user = Receptionists.FirstOrDefault(r => r.Id == id && r.Password == password);
            if (user != null)
            {
                Console.WriteLine($"Receptionist login successful: {user.FirstName} {user.LastName}");
                return user;
            }

            Console.WriteLine($"Login failed: No user found with ID {id} and provided password");
            return null;
        }

        /// <summary>
        /// Add a new patient to the system
        /// </summary>
        /// <param name="patient">Patient to add</param>
        public void AddPatient(Patient patient)
        {
            if (patient != null)
            {
                Patients.Add(patient);
                Console.WriteLine($"Patient {patient.FirstName} {patient.LastName} (ID: {patient.Id}) added to system");
                SaveAllData();
            }
            else
            {
                Console.WriteLine("Error: Cannot add null patient");
            }
        }

        /// <summary>
        /// Add a new doctor to the system
        /// </summary>
        /// <param name="doctor">Doctor to add</param>
        public void AddDoctor(Doctor doctor)
        {
            if (doctor != null)
            {
                Doctors.Add(doctor);
                Console.WriteLine($"Doctor {doctor.FirstName} {doctor.LastName} (ID: {doctor.Id}) added to system");
                SaveAllData();
            }
            else
            {
                Console.WriteLine("Error: Cannot add null doctor");
            }
        }

        /// <summary>
        /// Add a new administrator to the system
        /// </summary>
        /// <param name="administrator">Administrator to add</param>
        public void AddAdministrator(Administrator administrator)
        {
            if (administrator != null)
            {
                Administrators.Add(administrator);
                Console.WriteLine($"Administrator {administrator.FirstName} {administrator.LastName} (ID: {administrator.Id}) added to system");
                SaveAllData();
            }
            else
            {
                Console.WriteLine("Error: Cannot add null administrator");
            }
        }

        /// <summary>
        /// Add a new appointment to the system
        /// </summary>
        /// <param name="appointment">Appointment to add</param>
        public void AddAppointment(Appointment appointment)
        {
            if (appointment != null)
            {
                Appointments.Add(appointment);
                Console.WriteLine($"Appointment {appointment.AppointmentId} added to system");
                SaveAllData();
            }
            else
            {
                Console.WriteLine("Error: Cannot add null appointment");
            }
        }

        /// <summary>
        /// Remove patient from the system
        /// </summary>
        /// <param name="patientId">ID of patient to remove</param>
        /// <returns>True if patient was removed, false otherwise</returns>
        public bool RemovePatient(int patientId)
        {
            Patient patient = Patients.FirstOrDefault(p => p.Id == patientId);
            if (patient != null)
            {
                Patients.Remove(patient);
                Console.WriteLine($"Patient {patient.FirstName} {patient.LastName} (ID: {patientId}) removed from system");
                SaveAllData();
                return true;
            }
            Console.WriteLine($"Patient with ID {patientId} not found");
            return false;
        }

        /// <summary>
        /// Remove doctor from the system
        /// </summary>
        /// <param name="doctorId">ID of doctor to remove</param>
        /// <returns>True if doctor was removed, false otherwise</returns>
        public bool RemoveDoctor(int doctorId)
        {
            Doctor doctor = Doctors.FirstOrDefault(d => d.Id == doctorId);
            if (doctor != null)
            {
                Doctors.Remove(doctor);
                Console.WriteLine($"Doctor {doctor.FirstName} {doctor.LastName} (ID: {doctorId}) removed from system");
                SaveAllData();
                return true;
            }
            Console.WriteLine($"Doctor with ID {doctorId} not found");
            return false;
        }

        /// <summary>
        /// Get system statistics
        /// </summary>
        /// <returns>Formatted string with system statistics</returns>
        public string GetSystemStatistics()
        {
            return $"System Statistics:\n" +
                   $"- Total Patients: {Patients.Count}\n" +
                   $"- Total Doctors: {Doctors.Count}\n" +
                   $"- Total Administrators: {Administrators.Count}\n" +
                   $"- Total Appointments: {Appointments.Count}";
        }

        /// <summary>
        /// Validate data integrity across all collections
        /// </summary>
        /// <returns>True if data is consistent, false otherwise</returns>
        public bool ValidateDataIntegrity()
        {
            bool isValid = true;
            Console.WriteLine("=== Validating Data Integrity ===");

            // Check for duplicate IDs
            var allUsers = new List<User>();
            allUsers.AddRange(Patients);
            allUsers.AddRange(Doctors);
            allUsers.AddRange(Administrators);

            var duplicateIds = allUsers.GroupBy(u => u.Id)
                                     .Where(g => g.Count() > 1)
                                     .Select(g => g.Key);

            if (duplicateIds.Any())
            {
                Console.WriteLine($"ERROR: Duplicate user IDs found: {string.Join(", ", duplicateIds)}");
                isValid = false;
            }

            // Check appointment references
            foreach (var appointment in Appointments)
            {
                if (!Doctors.Any(d => d.Id == appointment.DoctorId))
                {
                    Console.WriteLine($"ERROR: Appointment {appointment.AppointmentId} references non-existent doctor ID {appointment.DoctorId}");
                    isValid = false;
                }

                if (!Patients.Any(p => p.Id == appointment.PatientId))
                {
                    Console.WriteLine($"ERROR: Appointment {appointment.AppointmentId} references non-existent patient ID {appointment.PatientId}");
                    isValid = false;
                }
            }

            Console.WriteLine(isValid ? "Data integrity check passed" : "Data integrity check failed");
            Console.WriteLine("=== Validation Complete ===");
            return isValid;
        }
    }
}