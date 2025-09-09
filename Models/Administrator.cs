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

        // Constructors
        public Administrator() : base()
        {
        }

        public Administrator(string firstName, string lastName) : base(firstName, lastName)
        {
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
            return $"Admin: {FirstName} {LastName} (ID: {Id}) ";
        }

    }
}
