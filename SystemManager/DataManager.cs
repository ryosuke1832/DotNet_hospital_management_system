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
            FileManager.InitializeDataFiles();
            LoadAllData();
            CreateSampleDataIfNeeded();
        }

        /// <summary>
        /// Load all data from files
        /// </summary>
        public void LoadAllData()
        {
            try
            {
                Patients = FileManager.LoadPatients();
                Doctors = FileManager.LoadDoctors();
                Administrators = FileManager.LoadAdministrators();
                Appointments = FileManager.LoadAppointments();

                Console.WriteLine($"Loaded: {Patients.Count} patients, {Doctors.Count} doctors, {Administrators.Count} administrators, {Appointments.Count} appointments");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
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
                Console.WriteLine("Creating sample data for testing...");

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

                // Save sample data
                SaveAllData();

                Console.WriteLine("Sample data created:");
                Console.WriteLine($"Administrator - ID: {sampleAdmin.Id}, Password: admin123");
                Console.WriteLine($"Doctor - ID: {sampleDoctor.Id}, Password: doctor123");
                Console.WriteLine($"Patient - ID: {samplePatient.Id}, Password: patient123");
            }
        }

        /// <summary>
        /// Find user by ID and password across all user types
        /// </summary>
        public User FindUser(int id, string password)
        {
            // Check patients
            User user = Patients.FirstOrDefault(p => p.Id == id && p.Password == password);
            if (user != null) return user;

            // Check doctors
            user = Doctors.FirstOrDefault(d => d.Id == id && d.Password == password);
            if (user != null) return user;

            // Check administrators
            user = Administrators.FirstOrDefault(a => a.Id == id && a.Password == password);
            if (user != null) return user;

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
