using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1_hospital_management_system.Models
{
    // Base class - Common properties and methods for all users
    public abstract class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }

        // Constructor base
        public User()
        {
            Id = GenerateId();
        }

        // Constructor add names
        public User(string firstName, string lastName) : this()
        {
            FirstName = firstName;
            LastName = lastName;
        }

        // Constructor add names and contact info
        public User(string firstName, string lastName, string email, string phone) : this(firstName, lastName)
        {
            Email = email;
            Phone = phone;
        }

        // Abstract methods - must be overridden in derived classes
        public abstract void ShowMainMenu();
        public abstract string GetUserType();

        // Virtual method - can be overridden
        public virtual string ToString()
        {
            return $"{FirstName} {LastName} (ID: {Id})";
        }

        // Common method for authentication
        public bool Login(int id, string password)
        {
            return Id == id && Password == password;
        }

        // Virtual method for ID generation
        protected virtual int GenerateId()
        {
            Random random = new Random();
            return random.Next(10000, 99999);
        }
    }
}
