using Assignment1_hospital_management_system.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Assignment1_hospital_management_system.Utilities
{

    // FileManager class - handles all file operations
    public static class FileManager
    {
        // File paths for data storage
        public const string PATIENTS_FILE = "patients.txt";
        public const string DOCTORS_FILE = "doctors.txt";
        public const string ADMINISTRATORS_FILE = "administrators.txt";
        public const string APPOINTMENTS_FILE = "appointments.txt";

        // Initialize data files if they don't exist
        public static void InitializeDataFiles()
        {
            try
            {
                CreateFileIfNotExists(PATIENTS_FILE);
                CreateFileIfNotExists(DOCTORS_FILE);
                CreateFileIfNotExists(ADMINISTRATORS_FILE);
                CreateFileIfNotExists(APPOINTMENTS_FILE);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing data files: {ex.Message}");
            }
        }

        // Create file if it doesn't exist
        private static void CreateFileIfNotExists(string fileName)
        {
            if (!File.Exists(fileName))
            {
                File.Create(fileName).Dispose();
                Console.WriteLine($"Created data file: {fileName}");
            }
        }

        // Save patients to file
        public static void SavePatients(List<Patient> patients)
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (Patient patient in patients)
                {
                    string line = $"{patient.Id},{patient.FirstName},{patient.LastName},{patient.Email},{patient.Phone},{patient.Address},{patient.Password},{patient.MedicalHistory},{patient.AssignedDoctorId}";
                    lines.Add(line);
                }
                File.WriteAllLines(PATIENTS_FILE, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving patients: {ex.Message}");
            }
        }

        // Load patients from file
        public static List<Patient> LoadPatients()
        {
            List<Patient> patients = new List<Patient>();
            try
            {
                if (File.Exists(PATIENTS_FILE))
                {
                    string[] lines = File.ReadAllLines(PATIENTS_FILE);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            Patient patient = ParsePatientFromLine(line);
                            if (patient != null)
                            {
                                patients.Add(patient);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading patients: {ex.Message}");
            }
            return patients;
        }

        // Parse patient from file line
        private static Patient ParsePatientFromLine(string line)
        {
            try
            {
                string[] parts = line.Split(',');
                if (parts.Length >= 9)
                {
                    Patient patient = new Patient
                    {
                        Id = int.Parse(parts[0]),
                        FirstName = parts[1],
                        LastName = parts[2],
                        Email = parts[3],
                        Phone = parts[4],
                        Address = parts[5],
                        Password = parts[6],
                        MedicalHistory = parts[7]
                    };

                    if (int.TryParse(parts[8], out int doctorId))
                    {
                        patient.AssignedDoctorId = doctorId;
                    }

                    return patient;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing patient data: {ex.Message}");
            }
            return null;
        }

        // Save doctors to file
        public static void SaveDoctors(List<Doctor> doctors)
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (Doctor doctor in doctors)
                {
                    string patientIds = string.Join(";", doctor.PatientIds);
                    string line = $"{doctor.Id},{doctor.FirstName},{doctor.LastName},{doctor.Email},{doctor.Phone},{doctor.Address},{doctor.Password},{doctor.Specialization},{patientIds}";
                    lines.Add(line);
                }
                File.WriteAllLines(DOCTORS_FILE, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving doctors: {ex.Message}");
            }
        }

        // Load doctors from file
        public static List<Doctor> LoadDoctors()
        {
            List<Doctor> doctors = new List<Doctor>();
            try
            {
                if (File.Exists(DOCTORS_FILE))
                {
                    string[] lines = File.ReadAllLines(DOCTORS_FILE);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            Doctor doctor = ParseDoctorFromLine(line);
                            if (doctor != null)
                            {
                                doctors.Add(doctor);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading doctors: {ex.Message}");
            }
            return doctors;
        }

        // Parse doctor from file line
        private static Doctor ParseDoctorFromLine(string line)
        {
            try
            {
                string[] parts = line.Split(',');
                if (parts.Length >= 9)
                {
                    Doctor doctor = new Doctor
                    {
                        Id = int.Parse(parts[0]),
                        FirstName = parts[1],
                        LastName = parts[2],
                        Email = parts[3],
                        Phone = parts[4],
                        Address = parts[5],
                        Password = parts[6],
                        Specialization = parts[7]
                    };

                    // Parse patient IDs
                    if (!string.IsNullOrWhiteSpace(parts[8]))
                    {
                        string[] patientIdStrings = parts[8].Split(';');
                        foreach (string idString in patientIdStrings)
                        {
                            if (int.TryParse(idString, out int patientId))
                            {
                                doctor.PatientIds.Add(patientId);
                            }
                        }
                    }

                    return doctor;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing doctor data: {ex.Message}");
            }
            return null;
        }

        // Save administrators to file
        public static void SaveAdministrators(List<Administrator> administrators)
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (Administrator admin in administrators)
                {
                    string line = $"{admin.Id},{admin.FirstName},{admin.LastName},{admin.Email},{admin.Phone},{admin.Address},{admin.Password},{admin.Department},{admin.AccessLevel}";
                    lines.Add(line);
                }
                File.WriteAllLines(ADMINISTRATORS_FILE, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving administrators: {ex.Message}");
            }
        }

        // Load administrators from file
        public static List<Administrator> LoadAdministrators()
        {
            List<Administrator> administrators = new List<Administrator>();
            try
            {
                if (File.Exists(ADMINISTRATORS_FILE))
                {
                    string[] lines = File.ReadAllLines(ADMINISTRATORS_FILE);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            Administrator admin = ParseAdministratorFromLine(line);
                            if (admin != null)
                            {
                                administrators.Add(admin);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading administrators: {ex.Message}");
            }
            return administrators;
        }

        // Parse administrator from file line
        private static Administrator ParseAdministratorFromLine(string line)
        {
            try
            {
                string[] parts = line.Split(',');
                if (parts.Length >= 9)
                {
                    return new Administrator
                    {
                        Id = int.Parse(parts[0]),
                        FirstName = parts[1],
                        LastName = parts[2],
                        Email = parts[3],
                        Phone = parts[4],
                        Address = parts[5],
                        Password = parts[6],
                        Department = parts[7],
                        AccessLevel = parts[8]
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing administrator data: {ex.Message}");
            }
            return null;
        }

        // Save appointments to file
        public static void SaveAppointments(List<Appointment> appointments)
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (Appointment appointment in appointments)
                {
                    lines.Add(appointment.ToFileString());
                }
                File.WriteAllLines(APPOINTMENTS_FILE, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving appointments: {ex.Message}");
            }
        }

        // Load appointments from file
        public static List<Appointment> LoadAppointments()
        {
            List<Appointment> appointments = new List<Appointment>();
            try
            {
                if (File.Exists(APPOINTMENTS_FILE))
                {
                    string[] lines = File.ReadAllLines(APPOINTMENTS_FILE);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            Appointment appointment = Appointment.FromFileString(line);
                            if (appointment != null)
                            {
                                appointments.Add(appointment);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading appointments: {ex.Message}");
            }
            return appointments;
        }

        // Save all data to files
        public static void SaveAllData(List<Patient> patients, List<Doctor> doctors, List<Administrator> administrators, List<Appointment> appointments)
        {
            SavePatients(patients);
            SaveDoctors(doctors);
            SaveAdministrators(administrators);
            SaveAppointments(appointments);
            Console.WriteLine("All data saved successfully!");
        }
    }
}
