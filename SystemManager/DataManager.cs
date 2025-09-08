using Assignment1_hospital_management_system.Models;
using Assignment1_hospital_management_system.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment1_hospital_management_system.SystemManager
{
    /// <summary>
    /// 簡略化されたデータマネージャー - サンプルデータ作成ロジックを統合
    /// </summary>
    public class DataManager
    {
        private HashSet<int> usedIds = new HashSet<int>();
        private Random random = new Random();

        public List<Patient> Patients { get; private set; } = new List<Patient>();
        public List<Doctor> Doctors { get; private set; } = new List<Doctor>();
        public List<Administrator> Administrators { get; private set; } = new List<Administrator>();
        public List<Appointment> Appointments { get; private set; } = new List<Appointment>();
        public List<Receptionist> Receptionists { get; private set; } = new List<Receptionist>();

        public void Initialize()
        {
            Console.WriteLine("=== Initializing Data Manager ===");
            FileManager.InitializeDataFiles();
            LoadAllData();
            CreateSampleDataIfNeeded();
            Console.WriteLine("=== Data Manager initialized successfully ===");
        }

        /// <summary>
        /// サンプルデータ作成 - 統合された簡略版
        /// </summary>
        private void CreateSampleDataIfNeeded()
        {
            bool dataCreated = false;

            // 管理者作成
            if (!Administrators.Any())
            {
                Administrators.Add(CreateUser<Administrator>("System", "Administrator", new
                {
                    Email = "admin@hospital.com",
                    Password = "admin1234",
                    Department = "Administration",
                    AccessLevel = "Full Access"
                }));
                dataCreated = true;
            }

            // 医師作成  
            if (!Doctors.Any())
            {
                var doctorData = new[]
                {
                    new { FirstName = "John", LastName = "Smith", Specialization = "Cardiology" },
                    new { FirstName = "Sarah", LastName = "Johnson", Specialization = "Pediatrics" }
                };

                foreach (var data in doctorData)
                {
                    Doctors.Add(CreateUser<Doctor>(data.FirstName, data.LastName, new
                    {
                        Specialization = data.Specialization,
                        Password = "doctor123",
                        Email = $"{data.FirstName.ToLower()}.{data.LastName.ToLower()}@hospital.com"
                    }));
                }
                dataCreated = true;
            }

            // 患者作成
            if (!Patients.Any())
            {
                var patientData = new[]
                {
                    new { FirstName = "Mike", LastName = "Wilson", MedicalHistory = "No significant history" },
                    new { FirstName = "Emily", LastName = "Brown", MedicalHistory = "Allergic to penicillin" },
                    new { FirstName = "David", LastName = "Taylor", MedicalHistory = "Diabetes Type 2" }
                };

                foreach (var data in patientData)
                {
                    Patients.Add(CreateUser<Patient>(data.FirstName, data.LastName, new
                    {
                        MedicalHistory = data.MedicalHistory,
                        Password = "patient123",
                        Email = $"{data.FirstName.ToLower()}.{data.LastName.ToLower()}@email.com"
                    }));
                }
                dataCreated = true;
            }

            // 受付嬢作成
            if (!Receptionists.Any())
            {
                Receptionists.Add(CreateUser<Receptionist>("Lisa", "Anderson", new
                {
                    Password = "reception123",
                    Email = "lisa.anderson@hospital.com"
                }));
                dataCreated = true;
            }

            // 予約作成とアサイン
            if (!Appointments.Any() && Doctors.Any() && Patients.Any())
            {
                for (int i = 0; i < Math.Min(Doctors.Count, Patients.Count); i++)
                {
                    var doctor = Doctors[i];
                    var patient = Patients[i];

                    patient.AssignedDoctorId = doctor.Id;
                    doctor.AddPatient(patient.Id);

                    Appointments.Add(new Appointment(doctor.Id, patient.Id,
                        i == 0 ? "Regular health checkup" :
                        i == 1 ? "Pediatric consultation" : "Follow-up visit"));
                }
                dataCreated = true;
            }

            if (dataCreated) SaveAllData();
        }

        /// <summary>
        /// Generic user creation helper - リフレクションを使って簡略化
        /// </summary>
        private T CreateUser<T>(string firstName, string lastName, object additionalProperties) where T : User, new()
        {
            var user = new T
            {
                FirstName = firstName,
                LastName = lastName,
                Phone = $"04{random.Next(10000000, 99999999)}",
                Address = "123 Sample Street, Sydney, NSW"
            };

            // additionalPropertiesのプロパティを設定
            var userType = typeof(T);
            var props = additionalProperties.GetType().GetProperties();

            foreach (var prop in props)
            {
                var userProp = userType.GetProperty(prop.Name);
                if (userProp != null && userProp.CanWrite)
                {
                    userProp.SetValue(user, prop.GetValue(additionalProperties));
                }
            }

            Console.WriteLine($"Created {typeof(T).Name}: {firstName} {lastName} (ID: {user.Id})");
            return user;
        }

        public void LoadAllData()
        {
            try
            {
                Patients = FileManager.LoadPatients();
                Doctors = FileManager.LoadDoctors();
                Administrators = FileManager.LoadAdministrators();
                Appointments = FileManager.LoadAppointments();
                Receptionists = FileManager.LoadReceptionists();

                Console.WriteLine($"Loaded: {Patients.Count} patients, {Doctors.Count} doctors, " +
                    $"{Administrators.Count} admins, {Appointments.Count} appointments, {Receptionists.Count} receptionists");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        public int GenerateUniqueId()
        {
            int newId;
            int attempts = 0;
            const int maxAttempts = 1000;

            do
            {
                newId = random.Next(10000, 99999);
                if (++attempts >= maxAttempts)
                    throw new InvalidOperationException("Failed to generate unique ID");
            }
            while (usedIds.Contains(newId));

            usedIds.Add(newId);
            return newId;
        }

        public void RegisterExistingIds()
        {
            var allUsers = new List<User>();
            allUsers.AddRange(Patients);
            allUsers.AddRange(Doctors);
            allUsers.AddRange(Administrators);
            allUsers.AddRange(Receptionists);

            foreach (var user in allUsers)
                usedIds.Add(user.Id);

            foreach (var appointment in Appointments)
                usedIds.Add(appointment.AppointmentId);
        }

        /// <summary>
        /// ガベージコレクション example - 課題要件
        /// </summary>
        public void ShowMemoryUsageAndCleanup()
        {
            Console.WriteLine("=== Memory Management ===");

            long memoryBefore = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory before GC: {memoryBefore / 1024:N0} KB");

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long memoryAfter = GC.GetTotalMemory(true);
            Console.WriteLine($"Memory after GC: {memoryAfter / 1024:N0} KB");
            Console.WriteLine($"Memory freed: {(memoryBefore - memoryAfter) / 1024:N0} KB");
        }

        public void SaveAllData()
        {
            try
            {
                FileManager.SaveAllData(Patients, Doctors, Administrators, Appointments, Receptionists);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }

        public User FindUser(int id, string password)
        {
            var allUsers = new List<User>();
            allUsers.AddRange(Patients);
            allUsers.AddRange(Doctors);
            allUsers.AddRange(Administrators);
            allUsers.AddRange(Receptionists);

            return allUsers.FirstOrDefault(u => u.Id == id && u.Password == password);
        }

        // Generic add methods
        public void AddUser<T>(T user) where T : User
        {
            switch (user)
            {
                case Patient p: Patients.Add(p); break;
                case Doctor d: Doctors.Add(d); break;
                case Administrator a: Administrators.Add(a); break;
                case Receptionist r: Receptionists.Add(r); break;
            }
            SaveAllData();
        }

        // Specific add methods for backward compatibility
        public void AddPatient(Patient patient) => AddUser(patient);
        public void AddDoctor(Doctor doctor) => AddUser(doctor);
        public void AddAdministrator(Administrator admin) => AddUser(admin);
        public void AddReceptionist(Receptionist receptionist) => AddUser(receptionist);

        public void AddAppointment(Appointment appointment)
        {
            Appointments.Add(appointment);
            SaveAllData();
        }

        public string GetSystemStatistics() =>
            $"Patients: {Patients.Count}, Doctors: {Doctors.Count}, " +
            $"Admins: {Administrators.Count}, Appointments: {Appointments.Count}, " +
            $"Receptionists: {Receptionists.Count}";
    }
}