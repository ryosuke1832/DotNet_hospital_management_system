using Assignment1_hospital_management_system.Models;

namespace Assignment1_hospital_management_system.Utilities
{
    /// <summary>
    /// FileManager class - handles all file operations with Data folder structure
    /// </summary>
    public static class FileManager
    {
        // Get project root directory (where .csproj file is located)
        private static string GetProjectRootDirectory()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo directory = new DirectoryInfo(currentDirectory);

            // Walk up the directory tree to find the project root (containing .csproj file)
            while (directory != null && !directory.GetFiles("*.csproj").Any())
            {
                directory = directory.Parent;
            }

            // If project root found, use it; otherwise use current directory
            return directory?.FullName ?? currentDirectory;
        }

        // Data folder path - now uses project root
        public static readonly string DATA_FOLDER = Path.Combine(GetProjectRootDirectory(), "Data");

        // File paths for data storage within Data folder
        public static readonly string PATIENTS_FILE = Path.Combine(DATA_FOLDER, "patients.txt");
        public static readonly string DOCTORS_FILE = Path.Combine(DATA_FOLDER, "doctors.txt");
        public static readonly string ADMINISTRATORS_FILE = Path.Combine(DATA_FOLDER, "administrators.txt");
        public static readonly string APPOINTMENTS_FILE = Path.Combine(DATA_FOLDER, "appointments.txt");
        public static readonly string RECEPTIONISTS_FILE = Path.Combine(DATA_FOLDER, "receptionists.txt");

        /// <summary>
        /// Initialize data files and create Data folder if they don't exist
        /// </summary>
        public static void InitializeDataFiles()
        {
            try
            {
                // Create Data folder if it doesn't exist
                if (!Directory.Exists(DATA_FOLDER))
                {
                    Directory.CreateDirectory(DATA_FOLDER);
                }

                // Create individual data files with empty content if they don't exist
                CreateFileIfNotExists(PATIENTS_FILE);
                CreateFileIfNotExists(DOCTORS_FILE);
                CreateFileIfNotExists(ADMINISTRATORS_FILE);
                CreateFileIfNotExists(APPOINTMENTS_FILE);
                CreateFileIfNotExists(RECEPTIONISTS_FILE);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing data files: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Create empty file if it doesn't exist
        /// </summary>
        private static void CreateFileIfNotExists(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, "");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating file {filePath}: {ex.Message}");
            }
        }

        /// <summary>
        /// Save patients to file with pipe delimiter
        /// </summary>
        public static void SavePatients(List<Patient> patients)
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (Patient patient in patients)
                {
                    // Format: ID|FirstName|LastName|Email|Phone|Address|Password|AssignedDoctorId
                    string line = $"{patient.Id}|{CleanText(patient.FirstName)}|{CleanText(patient.LastName)}|{CleanText(patient.Email)}|{CleanText(patient.Phone)}|{CleanText(patient.Address)}|{CleanText(patient.Password)}|{patient.AssignedDoctorId?.ToString() ?? ""}";
                    lines.Add(line);
                }
                File.WriteAllLines(PATIENTS_FILE, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving patients: {ex.Message}");
            }
        }

        /// <summary>
        /// Load patients from file with pipe delimiter
        /// </summary>
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

        /// <summary>
        /// Parse patient from file line with pipe delimiter
        /// </summary>
        private static Patient ParsePatientFromLine(string line)
        {
            try
            {
                string[] parts = line.Split('|');
                if (parts.Length >= 8)
                {
                    Patient patient = new Patient
                    {
                        Id = int.Parse(parts[0]),
                        FirstName = parts[1] ?? "",
                        LastName = parts[2] ?? "",
                        Email = parts[3] ?? "",
                        Phone = parts[4] ?? "",
                        Address = parts[5] ?? "",
                        Password = parts[6] ?? ""
                    };

                    // Handle AssignedDoctorId which might be empty
                    if (!string.IsNullOrEmpty(parts[7]) && int.TryParse(parts[7], out int doctorId))
                    {
                        patient.AssignedDoctorId = doctorId;
                    }

                    return patient;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing patient data: {ex.Message} - Line: {line}");
            }
            return null;
        }

        /// <summary>
        /// Save doctors to file with pipe delimiter
        /// </summary>
        public static void SaveDoctors(List<Doctor> doctors)
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (Doctor doctor in doctors)
                {
                    // Format patient IDs as semicolon-separated string
                    string patientIds = string.Join(";", doctor.PatientIds);
                    string line = $"{doctor.Id}|{CleanText(doctor.FirstName)}|{CleanText(doctor.LastName)}|{CleanText(doctor.Email)}|{CleanText(doctor.Phone)}|{CleanText(doctor.Address)}|{CleanText(doctor.Password)}|{CleanText(doctor.Specialization)}|{patientIds}";
                    lines.Add(line);
                }
                File.WriteAllLines(DOCTORS_FILE, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving doctors: {ex.Message}");
            }
        }

        /// <summary>
        /// Load doctors from file with pipe delimiter
        /// </summary>
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

        /// <summary>
        /// Parse doctor from file line with pipe delimiter
        /// </summary>
        private static Doctor ParseDoctorFromLine(string line)
        {
            try
            {
                string[] parts = line.Split('|');
                if (parts.Length >= 9)
                {
                    Doctor doctor = new Doctor
                    {
                        Id = int.Parse(parts[0]),
                        FirstName = parts[1] ?? "",
                        LastName = parts[2] ?? "",
                        Email = parts[3] ?? "",
                        Phone = parts[4] ?? "",
                        Address = parts[5] ?? "",
                        Password = parts[6] ?? "",
                        Specialization = parts[7] ?? ""
                    };

                    // Parse patient IDs if they exist
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
                Console.WriteLine($"Error parsing doctor data: {ex.Message} - Line: {line}");
            }
            return null;
        }

        /// <summary>
        /// Save administrators to file with pipe delimiter
        /// </summary>
        public static void SaveAdministrators(List<Administrator> administrators)
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (Administrator admin in administrators)
                {
                    string line = $"{admin.Id}|{CleanText(admin.FirstName)}|{CleanText(admin.LastName)}|{CleanText(admin.Email)}|{CleanText(admin.Phone)}|{CleanText(admin.Address)}|{CleanText(admin.Password)}";
                    lines.Add(line);
                }
                File.WriteAllLines(ADMINISTRATORS_FILE, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving administrators: {ex.Message}");
            }
        }

        /// <summary>
        /// Load administrators from file with pipe delimiter
        /// </summary>
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

        /// <summary>
        /// Parse administrator from file line with pipe delimiter
        /// </summary>
        private static Administrator ParseAdministratorFromLine(string line)
        {
            try
            {
                string[] parts = line.Split('|');
                if (parts.Length >= 7)
                {
                    return new Administrator
                    {
                        Id = int.Parse(parts[0]),
                        FirstName = parts[1] ?? "",
                        LastName = parts[2] ?? "",
                        Email = parts[3] ?? "",
                        Phone = parts[4] ?? "",
                        Address = parts[5] ?? "",
                        Password = parts[6] ?? ""
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing administrator data: {ex.Message} - Line: {line}");
            }
            return null;
        }

        /// <summary>
        /// Save receptionists to file with pipe delimiter
        /// </summary>
        public static void SaveReceptionists(List<Receptionist> receptionists)
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (Receptionist receptionist in receptionists)
                {
                    string line = $"{receptionist.Id}|{CleanText(receptionist.FirstName)}|{CleanText(receptionist.LastName)}|{CleanText(receptionist.Email)}|{CleanText(receptionist.Phone)}|{CleanText(receptionist.Address)}|{CleanText(receptionist.Password)}";
                    lines.Add(line);
                }
                File.WriteAllLines(RECEPTIONISTS_FILE, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving receptionists: {ex.Message}");
            }
        }

        /// <summary>
        /// Load receptionists from file with pipe delimiter
        /// </summary>
        public static List<Receptionist> LoadReceptionists()
        {
            List<Receptionist> receptionists = new List<Receptionist>();
            try
            {
                if (File.Exists(RECEPTIONISTS_FILE))
                {
                    string[] lines = File.ReadAllLines(RECEPTIONISTS_FILE);

                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            Receptionist receptionist = ParseReceptionistFromLine(line);
                            if (receptionist != null)
                            {
                                receptionists.Add(receptionist);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading receptionists: {ex.Message}");
            }
            return receptionists;
        }

        /// <summary>
        /// Parse receptionist from file line with pipe delimiter
        /// </summary>
        private static Receptionist ParseReceptionistFromLine(string line)
        {
            try
            {
                string[] parts = line.Split('|');
                if (parts.Length >= 7)
                {
                    return new Receptionist
                    {
                        Id = int.Parse(parts[0]),
                        FirstName = parts[1] ?? "",
                        LastName = parts[2] ?? "",
                        Email = parts[3] ?? "",
                        Phone = parts[4] ?? "",
                        Address = parts[5] ?? "",
                        Password = parts[6] ?? ""
                    };
                }
                else
                {
                    Console.WriteLine($"Invalid receptionist data format: {line}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing receptionist data: {ex.Message} - Line: {line}");
            }
            return null;
        }

        /// <summary>
        /// Save appointments to file with pipe delimiter
        /// </summary>
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

        /// <summary>
        /// Load appointments from file with pipe delimiter
        /// </summary>
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

        /// <summary>
        /// Save all data to files
        /// </summary>
        public static void SaveAllData(List<Patient> patients, List<Doctor> doctors, List<Administrator> administrators, List<Appointment> appointments, List<Receptionist> receptionists)
        {
            Console.WriteLine("=== Saving all data to files ===");
            SavePatients(patients);
            SaveDoctors(doctors);
            SaveAdministrators(administrators);
            SaveAppointments(appointments);
            SaveReceptionists(receptionists);
            Console.WriteLine("=== All data saved successfully! ===");
        }

        /// <summary>
        /// Clean text to remove any pipe characters that might interfere with parsing
        /// </summary>
        private static string CleanText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // Replace pipe characters with dash to prevent parsing issues
            return text.Replace("|", "-");
        }
    }
}