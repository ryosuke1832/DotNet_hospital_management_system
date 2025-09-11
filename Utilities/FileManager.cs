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
                else
                {
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
        /// Save patients to file with error handling and logging
        /// </summary>
        public static void SavePatients(List<Patient> patients)
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (Patient patient in patients)
                {
                    // Format: ID,FirstName,LastName,Email,Phone,Address,Password,MedicalHistory,AssignedDoctorId
                    string line = $"{patient.Id},{EscapeCommas(patient.FirstName)},{EscapeCommas(patient.LastName)},{EscapeCommas(patient.Email)},{EscapeCommas(patient.Phone)},{EscapeCommas(patient.Address)},{EscapeCommas(patient.Password)},{patient.AssignedDoctorId}";
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
        /// Load patients from file with improved error handling
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
        /// Parse patient from file line with improved error handling
        /// </summary>
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
                        FirstName = UnescapeCommas(parts[1]),
                        LastName = UnescapeCommas(parts[2]),
                        Email = UnescapeCommas(parts[3]),
                        Phone = UnescapeCommas(parts[4]),
                        Address = UnescapeCommas(parts[5]),
                        Password = UnescapeCommas(parts[6]),
                    };

                    // Handle AssignedDoctorId which might be null
                    if (int.TryParse(parts[8], out int doctorId) && doctorId > 0)
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
        /// Save doctors to file with improved formatting
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
                    string line = $"{doctor.Id},{EscapeCommas(doctor.FirstName)},{EscapeCommas(doctor.LastName)},{EscapeCommas(doctor.Email)},{EscapeCommas(doctor.Phone)},{EscapeCommas(doctor.Address)},{EscapeCommas(doctor.Password)},{EscapeCommas(doctor.Specialization)},{patientIds}";
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
        /// Load doctors from file
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
        /// Parse doctor from file line with improved error handling
        /// </summary>
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
                        FirstName = UnescapeCommas(parts[1]),
                        LastName = UnescapeCommas(parts[2]),
                        Email = UnescapeCommas(parts[3]),
                        Phone = UnescapeCommas(parts[4]),
                        Address = UnescapeCommas(parts[5]),
                        Password = UnescapeCommas(parts[6]),
                        Specialization = UnescapeCommas(parts[7])
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
        /// Save administrators to file
        /// </summary>
        public static void SaveAdministrators(List<Administrator> administrators)
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (Administrator admin in administrators)
                {
                    string line = $"{admin.Id},{EscapeCommas(admin.FirstName)},{EscapeCommas(admin.LastName)},{EscapeCommas(admin.Email)},{EscapeCommas(admin.Phone)},{EscapeCommas(admin.Address)},{EscapeCommas(admin.Password)}";
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
        /// Load administrators from file
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
        /// Parse administrator from file line
        /// </summary>
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
                        FirstName = UnescapeCommas(parts[1]),
                        LastName = UnescapeCommas(parts[2]),
                        Email = UnescapeCommas(parts[3]),
                        Phone = UnescapeCommas(parts[4]),
                        Address = UnescapeCommas(parts[5]),
                        Password = UnescapeCommas(parts[6]),
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
        /// save receptionists to file
        /// </summary>
        public static void SaveReceptionists(List<Receptionist> receptionists)
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (Receptionist receptionist in receptionists)
                {
                    // Format: ID,FirstName,LastName,Email,Phone,Address,Password,Department,Shift
                    string line = $"{receptionist.Id},{EscapeCommas(receptionist.FirstName)},{EscapeCommas(receptionist.LastName)},{EscapeCommas(receptionist.Email)},{EscapeCommas(receptionist.Phone)},{EscapeCommas(receptionist.Address)},{EscapeCommas(receptionist.Password)}";
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
        /// load receptionists from file
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
        /// analyze receptionist from file line
        /// </summary>
        private static Receptionist ParseReceptionistFromLine(string line)
        {
            try
            {
                string[] parts = line.Split(',');
                if (parts.Length >= 9)
                {
                    return new Receptionist
                    {
                        Id = int.Parse(parts[0]),
                        FirstName = UnescapeCommas(parts[1]),
                        LastName = UnescapeCommas(parts[2]),
                        Email = UnescapeCommas(parts[3]),
                        Phone = UnescapeCommas(parts[4]),
                        Address = UnescapeCommas(parts[5]),
                        Password = UnescapeCommas(parts[6]),
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
        /// Save appointments to file
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
        /// Load appointments from file
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
        /// Save all data to files with comprehensive logging
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
        /// Escape commas in text fields to prevent CSV parsing issues
        /// </summary>
        private static string EscapeCommas(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text.Replace(",", "&#44;"); // Replace commas with HTML entity
        }

        /// <summary>
        /// Unescape commas in text fields when reading from file
        /// </summary>
        private static string UnescapeCommas(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text.Replace("&#44;", ","); // Replace HTML entity back to commas
        }

    }
}