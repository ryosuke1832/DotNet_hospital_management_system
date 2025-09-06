using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1_hospital_management_system.Models
{

    // Administrator class
    public class Administrator : User
    {
        public string Department { get; set; }
        public string AccessLevel { get; set; }

        // Constructors
        public Administrator() : base()
        {
            AccessLevel = "Full Access";
            Department = "Administration";
        }

        public Administrator(string firstName, string lastName) : base(firstName, lastName)
        {
            AccessLevel = "Full Access";
            Department = "Administration";
        }

        // Abstract method override - Administrator menu
        public override void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("         Administrator Menu");
            Console.WriteLine("========================================");
            Console.WriteLine("Welcome to DOTNET Hospital Management System");
            Console.WriteLine();
            Console.WriteLine("Please choose an option:");
            Console.WriteLine("1. List all doctors");
            Console.WriteLine("2. Check doctor details");
            Console.WriteLine("3. List all patients");
            Console.WriteLine("4. Check patient details");
            Console.WriteLine("5. Add doctor");
            Console.WriteLine("6. Add patient");
            Console.WriteLine("7. Add receptionist");
            Console.WriteLine("8. Show filtered patients"); 
            Console.WriteLine("9. Show system statistics");
            Console.WriteLine("10. Logout");
            Console.WriteLine("11. Exit");
            Console.WriteLine("========================================");
        }

        public override string GetUserType()
        {
            return "Administrator";
        }

        // Method override - Administrator-specific display
        public override string ToString()
        {
            return $"Admin: {FirstName} {LastName} (ID: {Id}) | Department: {Department} | Access: {AccessLevel}";
        }


        // Administrator-specific methods - Method overloading examples
        public void AddUser(Patient patient)
        {
            Console.WriteLine($"Patient {patient.FirstName} {patient.LastName} has been successfully added to the system!");
            Console.WriteLine($"Generated Patient ID: {patient.Id}");
            // File saving logic would go here
        }

        public void AddUser(Doctor doctor)
        {
            Console.WriteLine($"Doctor {doctor.FirstName} {doctor.LastName} has been successfully added to the system!");
            Console.WriteLine($"Generated Doctor ID: {doctor.Id}");
            Console.WriteLine($"Specialization: {doctor.Specialization}");
            // File saving logic would go here
        }

        public void AddUser(Receptionist receptionist)
        {
            Console.WriteLine($"Receptionist {receptionist.FirstName} {receptionist.LastName} has been successfully added to the system!");
            Console.WriteLine($"Generated Receptionist ID: {receptionist.Id}");
        }

        public void AddUser(string userType, string firstName, string lastName)
        {
            switch (userType.ToLower())
            {
                case "patient":
                    AddUser(new Patient(firstName, lastName));
                    break;
                case "doctor":
                    AddUser(new Doctor(firstName, lastName, "General Practice"));
                    break;
                case "receptionist": 
                    AddUser(new Receptionist(firstName, lastName));
                    break;
                default:
                    Console.WriteLine("Invalid user type specified");
                    break;
            }
        }

        // Check user details by ID
        public void CheckUserDetails(int userId, string userType)
        {
            Console.WriteLine($"Searching for {userType} with ID: {userId}");
            // Logic to search and display user details would go here
        }
    }
}
