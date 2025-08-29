using Assignment1_hospital_management_system.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1_hospital_management_system.Models
{

    // Appointment class - represents appointments between doctors and patients
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }

        // Constructors
        public Appointment()
        {
            AppointmentId = Utils.GenerateId();
            CreatedDate = DateTime.Now;
            Status = "Scheduled";
        }

        public Appointment(int doctorId, int patientId, string description) : this()
        {
            DoctorId = doctorId;
            PatientId = patientId;
            Description = description;
        }

        // Override ToString for display purposes
        public override string ToString()
        {
            return $"Appointment ID: {AppointmentId} | Doctor ID: {DoctorId} | Patient ID: {PatientId} | Description: {Description} | Status: {Status}";
        }

        // Convert appointment to file format string
        public string ToFileString()
        {
            return $"{AppointmentId},{DoctorId},{PatientId},{Description},{CreatedDate:yyyy-MM-dd HH:mm:ss},{Status}";
        }

        // Create appointment from file format string
        public static Appointment FromFileString(string fileString)
        {
            try
            {
                string[] parts = fileString.Split(',');
                if (parts.Length >= 6)
                {
                    return new Appointment
                    {
                        AppointmentId = int.Parse(parts[0]),
                        DoctorId = int.Parse(parts[1]),
                        PatientId = int.Parse(parts[2]),
                        Description = parts[3],
                        CreatedDate = DateTime.Parse(parts[4]),
                        Status = parts[5]
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing appointment data: {ex.Message}");
            }
            return null;
        }
    }
}
