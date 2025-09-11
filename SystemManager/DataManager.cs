using Assignment1_hospital_management_system.Models;
using Assignment1_hospital_management_system.Utilities;

namespace Assignment1_hospital_management_system.SystemManager
{
    /// <summary>
    /// data manager - handles loading, saving, and managing all system data
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
            FileManager.InitializeDataFiles();
            LoadAllData();
            CreateSampleDataIfNeeded();

        }

        private void CreateSampleDataIfNeeded()
        {
            bool dataCreated = false;


            if (!Administrators.Any())
            {
                Administrators.Add(CreateUser<Administrator>("System", "Administrator", new
                {
                    Email = "admin@hospital.com",
                    Password = "admin1234",
                }));
                dataCreated = true;
            }


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


            if (!Receptionists.Any())
            {
                Receptionists.Add(CreateUser<Receptionist>("Lisa", "Anderson", new
                {
                    Password = "reception123",
                    Email = "lisa.anderson@hospital.com"
                }));
                dataCreated = true;
            }

 
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
        /// Generic user creation helper
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

            // additionalProperties
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
        /// Gabage collection and memory usage display
        /// </summary>
        public void ShowMemoryUsageAndCleanup()
        {

            long memoryBefore = GC.GetTotalMemory(false);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long memoryAfter = GC.GetTotalMemory(true);

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

    }
}