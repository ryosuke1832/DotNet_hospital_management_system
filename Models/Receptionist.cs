using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1_hospital_management_system.Models
{
    /// <summary>
    /// Receptionist class - responsible for hospital reception tasks
    /// Features: New patient registration, existing patient viewing, appointment list display, appointment addition
    /// </summary>
    public class Receptionist : User
    {
        public string Department { get; set; }
        public string Shift { get; set; }

        // Constructors
        public Receptionist() : base()
        {
            Department = "Reception";
            Shift = "Morning";
        }

        public Receptionist(string firstName, string lastName) : base(firstName, lastName)
        {
            Department = "Reception";
            Shift = "Morning";
        }

        // Abstract method override - Receptionist menu
        public override void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("         Receptionist Menu");
            Console.WriteLine("========================================");
            Console.WriteLine("Welcome to DOTNET Hospital Management System");
            Console.WriteLine();
            Console.WriteLine("Please choose an option:");
            Console.WriteLine("1. Register new patient");        // New patient registration
            Console.WriteLine("2. View existing patients");      // View existing patients
            Console.WriteLine("3. View appointments");           // View appointment list
            Console.WriteLine("4. Add new appointment");         // Add new appointment
            Console.WriteLine("5. Logout");
            Console.WriteLine("6. Exit");
            Console.WriteLine("========================================");
        }

        public override string GetUserType()
        {
            return "Receptionist";
        }

        // Method override - Receptionist-specific display
        public override string ToString()
        {
            return $"Receptionist: {FirstName} {LastName} (ID: {Id}) | Department: {Department}";
        }

        // Receptionist-specific methods - Method overloading examples

        /// <summary>
        /// Register new patient (Method overloading example)
        /// </summary>
        public Patient RegisterNewPatient(string firstName, string lastName, string phone)
        {
            Console.WriteLine($"Registering new patient: {firstName} {lastName}");
            Console.WriteLine($"Phone number: {phone}");

            Patient newPatient = new Patient(firstName, lastName)
            {
                Phone = phone,
                Email = $"{firstName.ToLower()}.{lastName.ToLower()}@email.com",
                Address = "Address not entered",
                Password = "patient123"
            };

            Console.WriteLine($"Registration completed with Patient ID {newPatient.Id}");
            return newPatient;
        }

        /// <summary>
        /// Register new patient (Full information version - Method overloading example)
        /// </summary>
        public Patient RegisterNewPatient(string firstName, string lastName, string phone, string email, string address)
        {
            Console.WriteLine($"Registering new patient: {firstName} {lastName}");

            Patient newPatient = new Patient(firstName, lastName)
            {
                Phone = phone,
                Email = email,
                Address = address,
                Password = "patient123"
            };

            Console.WriteLine($"Registration completed with Patient ID {newPatient.Id}");
            return newPatient;
        }

        /// <summary>
        /// Create appointment (Method overloading example)
        /// </summary>
        public Appointment CreateAppointment(int patientId, int doctorId, string description)
        {
            Console.WriteLine($"Creating appointment...");
            Console.WriteLine($"Patient ID: {patientId} | Doctor ID: {doctorId}");

            Appointment appointment = new Appointment(doctorId, patientId, description);
            Console.WriteLine($"Appointment created successfully with Appointment ID {appointment.AppointmentId}");
            return appointment;
        }

        /// <summary>
        /// Create appointment (Patient object version - Method overloading example)
        /// </summary>
        public Appointment CreateAppointment(Patient patient, Doctor doctor, string description)
        {
            return CreateAppointment(patient.Id, doctor.Id, description);
        }
    }
}