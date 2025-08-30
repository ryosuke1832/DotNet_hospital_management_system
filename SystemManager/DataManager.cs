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
        public List<Patient> Patients { get; private set; }
        public List<Doctor> Doctors { get; private set; }
        public List<Administrator> Administrators { get; private set; }
        public List<Appointment> Appointments { get; private set; }

        /// <summary>
        /// Constructor - initializes empty collections
        /// </summary>
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
            Console.WriteLine("=== Initializing Data Manager ===");

            // Initialize file system
            FileManager.InitializeDataFiles();

            // Display file information for debugging
            FileManager.DisplayFileInformation();

            // Load existing data
            LoadAllData();

            // Create sample data if needed
            CreateSampleDataIfNeeded();

            Console.WriteLine("=== Data Manager initialized successfully ===");
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
        /// Save all data to files with comprehensive logging
        /// </summary>
        public void SaveAllData()
        {
            try
            {
                Console.WriteLine("Saving all data to files...");
                FileManager.SaveAllData(Patients, Doctors, Administrators, Appointments);
                Console.WriteLine("Data save operation completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }

        /// <summary>
        /// Create sample data for testing if no data exists
        /// </summary>
        private void CreateSampleDataIfNeeded()
        {
            if (Patients.Count == 0 && Doctors.Count == 0 && Administrators.Count == 0)
            {
                Console.WriteLine("No existing data found. Creating sample data for testing...");

                // Create sample administrator
                Administrator sampleAdmin = new Administrator("David", "Adminson")
                {
                    Email = "admin@hospital.com",
                    Phone = "0412345678",
                    Address = "123 Admin Street, Sydney, NSW",
                    Password = "admin123",
                    Department = "Administration"
                };
                Administrators.Add(sampleAdmin);

                // Create sample doctor
                Doctor sampleDoctor = new Doctor("Jack", "Doctorson", "General Practice")
                {
                    Email = "jack@hospital.com",
                    Phone = "0412333676",
                    Address = "23 Real Street, Sydney, NSW",
                    Password = "doctor123"
                };
                Doctors.Add(sampleDoctor);

                // Create sample patient
                Patient samplePatient = new Patient("David", "Patientson")
                {
                    Email = "davey67@gmail.com",
                    Phone = "0412456876",
                    Address = "19 Real Street, Sydney, NSW",
                    Password = "patient123",
                    MedicalHistory = "No significant medical history",
                    AssignedDoctorId = sampleDoctor.Id
                };
                Patients.Add(samplePatient);

                // Assign patient to doctor
                sampleDoctor.AddPatient(samplePatient.Id);

                // Create sample appointment
                Appointment sampleAppointment = new Appointment(sampleDoctor.Id, samplePatient.Id, "Regular checkup with doctor");
                Appointments.Add(sampleAppointment);

                // Save sample data immediately
                Console.WriteLine("Saving sample data to files...");
                SaveAllData();

                // Display sample data for testing
                DisplaySampleDataInformation(sampleAdmin, sampleDoctor, samplePatient);
            }
            else
            {
                Console.WriteLine("Existing data found. Skipping sample data creation.");
            }
        }

        /// <summary>
        /// Display sample data information for testing purposes
        /// </summary>
        private void DisplaySampleDataInformation(Administrator admin, Doctor doctor, Patient patient)
        {
            Console.Clear();
            Utils.DisplayHeader("Sample Test Data Created");
            Console.WriteLine("The following test users have been created for demonstration:");
            Console.WriteLine();

            Console.WriteLine("=== ADMINISTRATOR ===");
            Console.WriteLine($"Name: {admin.FirstName} {admin.LastName}");
            Console.WriteLine($"ID: {admin.Id}");
            Console.WriteLine($"Password: admin123");
            Console.WriteLine($"Department: {admin.Department}");
            Console.WriteLine();

            Console.WriteLine("=== DOCTOR ===");
            Console.WriteLine($"Name: Dr. {doctor.FirstName} {doctor.LastName}");
            Console.WriteLine($"ID: {doctor.Id}");
            Console.WriteLine($"Password: doctor123");
            Console.WriteLine($"Specialization: {doctor.Specialization}");
            Console.WriteLine();

            Console.WriteLine("=== PATIENT ===");
            Console.WriteLine($"Name: {patient.FirstName} {patient.LastName}");
            Console.WriteLine($"ID: {patient.Id}");
            Console.WriteLine($"Password: patient123");
            Console.WriteLine($"Assigned Doctor: Dr. {doctor.FirstName} {doctor.LastName}");
            Console.WriteLine();

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