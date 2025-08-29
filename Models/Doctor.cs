using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1_hospital_management_system.Models
{

    // Doctor class
    public class Doctor : User
    {
        public string Specialization { get; set; }
        public List<int> PatientIds { get; set; }

        // Constructors
        public Doctor() : base()
        {
            PatientIds = new List<int>();
        }

        public Doctor(string firstName, string lastName, string specialization) : base(firstName, lastName)
        {
            Specialization = specialization;
            PatientIds = new List<int>();
        }

        // Abstract method override - Doctor menu
        public override void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("           Doctor Menu");
            Console.WriteLine("========================================");
            Console.WriteLine("Welcome to DOTNET Hospital Management System");
            Console.WriteLine();
            Console.WriteLine("Please choose an option:");
            Console.WriteLine("1. List doctor details");
            Console.WriteLine("2. List patients");
            Console.WriteLine("3. List appointments");
            Console.WriteLine("4. Check particular patient");
            Console.WriteLine("5. List appointments with patient");
            Console.WriteLine("6. Logout");
            Console.WriteLine("7. Exit");
            Console.WriteLine("========================================");
        }

        public override string GetUserType()
        {
            return "Doctor";
        }

        // Method override - Doctor-specific display
        public override string ToString()
        {
            return $"Dr. {FirstName} {LastName} (ID: {Id}) | Specialization: {Specialization} | Email: {Email}";
        }

        // Doctor-specific methods - Method overloading example
        public void AddPatient(int patientId)
        {
            if (!PatientIds.Contains(patientId))
            {
                PatientIds.Add(patientId);
                Console.WriteLine($"Patient with ID {patientId} has been assigned to Dr. {FirstName} {LastName}");
            }
            else
            {
                Console.WriteLine($"Patient with ID {patientId} is already assigned to this doctor");
            }
        }

        public void AddPatient(Patient patient)
        {
            AddPatient(patient.Id);
        }

        // Check patient details by ID
        public void CheckPatientDetails(int patientId)
        {
            if (PatientIds.Contains(patientId))
            {
                Console.WriteLine($"Patient ID {patientId} is registered with Dr. {FirstName} {LastName}");
            }
            else
            {
                Console.WriteLine($"No patient found with ID: {patientId}");
            }
        }
    }
}
