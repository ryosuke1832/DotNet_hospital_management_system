using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1_hospital_management_system.Models
{
    // Patient class
    public class Patient : User
    {
        public string MedicalHistory { get; set; }
        public int? AssignedDoctorId { get; set; }

        // Constructors
        public Patient() : base() { }

        public Patient(string firstName, string lastName) : base(firstName, lastName) { }

        // Abstract method override - Patient menu
        public override void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("           Patient Menu");
            Console.WriteLine("========================================");
            Console.WriteLine("Welcome to DOTNET Hospital Management System");
            Console.WriteLine();
            Console.WriteLine("Please choose an option:");
            Console.WriteLine("1. List patient details");
            Console.WriteLine("2. List my doctor details");
            Console.WriteLine("3. List all appointments");
            Console.WriteLine("4. Book appointment");
            Console.WriteLine("5. Exit to login");
            Console.WriteLine("6. Exit system");
            Console.WriteLine("========================================");
        }

        public override string GetUserType()
        {
            return "Patient";
        }

        // Method override - Patient-specific display
        public override string ToString()
        {
            return $"Patient: {FirstName} {LastName} (ID: {Id}) | Email: {Email} | Phone: {Phone}";
        }

    }

}
