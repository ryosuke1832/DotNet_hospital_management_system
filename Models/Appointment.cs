using Assignment1_hospital_management_system.SystemManager;


namespace Assignment1_hospital_management_system.Models
{
    /// <summary>
    /// Appointment class - represents appointments between doctors and patients
    /// </summary>
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        private static DataManager _dataManager;

        public static void SetDataManager(DataManager dataManager)
        {
            _dataManager = dataManager;
        }



        /// <summary>
        /// Default constructor - generates new appointment with unique ID
        /// </summary>
        public Appointment()
        {
            if (_dataManager != null)
            {
                AppointmentId = _dataManager.GenerateUniqueId();
            }
            else
            {
                throw new InvalidOperationException(
                    "DataManager must be set before creating Appointment objects. Call Appointment.SetDataManager() first.");
            }

            CreatedDate = DateTime.Now;
            Status = "Scheduled";
            Description = string.Empty;
        }

        /// <summary>
        /// Constructor with required appointment details
        /// </summary>
        public Appointment(int doctorId, int patientId, string description) : this()
        {
            DoctorId = doctorId;
            PatientId = patientId;
            Description = description ?? string.Empty;
        }

        /// <summary>
        /// Override ToString for display purposes
        /// </summary>
        public override string ToString()
        {
            return $"Appointment ID: {AppointmentId} | Doctor ID: {DoctorId} | Patient ID: {PatientId} | Description: {Description} | Status: {Status}";
        }

        /// <summary>
        /// Convert appointment to file format string with comma escaping
        /// Format: AppointmentId,DoctorId,PatientId,Description,CreatedDate,Status
        /// </summary>
        public string ToFileString()
        {
            // Escape commas in description and status to prevent CSV parsing issues
            string escapedDescription = EscapeCommas(Description);
            string escapedStatus = EscapeCommas(Status);

            return $"{AppointmentId},{DoctorId},{PatientId},{escapedDescription},{CreatedDate:yyyy-MM-dd HH:mm:ss},{escapedStatus}";
        }

        /// <summary>
        /// Create appointment from file format string with error handling
        /// </summary>
        public static Appointment FromFileString(string fileString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileString))
                {
                    Console.WriteLine("Empty or null appointment data provided");
                    return null;
                }

                string[] parts = fileString.Split(',');
                if (parts.Length >= 6)
                {
                    // Parse appointment data with validation
                    if (!int.TryParse(parts[0], out int appointmentId))
                    {
                        Console.WriteLine($"Invalid appointment ID: {parts[0]}");
                        return null;
                    }

                    if (!int.TryParse(parts[1], out int doctorId))
                    {
                        Console.WriteLine($"Invalid doctor ID: {parts[1]}");
                        return null;
                    }

                    if (!int.TryParse(parts[2], out int patientId))
                    {
                        Console.WriteLine($"Invalid patient ID: {parts[2]}");
                        return null;
                    }

                    if (!DateTime.TryParse(parts[4], out DateTime createdDate))
                    {
                        Console.WriteLine($"Invalid date format: {parts[4]}");
                        return null;
                    }

                    return new Appointment
                    {
                        AppointmentId = appointmentId,
                        DoctorId = doctorId,
                        PatientId = patientId,
                        Description = UnescapeCommas(parts[3]),
                        CreatedDate = createdDate,
                        Status = UnescapeCommas(parts[5])
                    };
                }
                else
                {
                    Console.WriteLine($"Invalid appointment data format - expected 6 fields, got {parts.Length}: {fileString}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing appointment data: {ex.Message} - Data: {fileString}");
            }
            return null;
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

        /// <summary>
        /// Update appointment status
        /// </summary>
        public void UpdateStatus(string newStatus)
        {
            if (!string.IsNullOrWhiteSpace(newStatus))
            {
                Status = newStatus;
                Console.WriteLine($"Appointment {AppointmentId} status updated to: {Status}");
            }
        }

        /// <summary>
        /// Update appointment description
        /// </summary>
        public void UpdateDescription(string newDescription)
        {
            Description = newDescription ?? string.Empty;
            Console.WriteLine($"Appointment {AppointmentId} description updated");
        }

        /// <summary>
        /// Get formatted appointment information for display
        /// </summary>
        public string GetFormattedInfo()
        {
            return $"Appointment #{AppointmentId}\n" +
                   $"Doctor ID: {DoctorId}\n" +
                   $"Patient ID: {PatientId}\n" +
                   $"Description: {Description}\n" +
                   $"Created: {CreatedDate:yyyy-MM-dd HH:mm}\n" +
                   $"Status: {Status}";
        }
    }
}