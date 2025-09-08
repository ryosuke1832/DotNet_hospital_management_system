using Assignment1_hospital_management_system.Models;
using Assignment1_hospital_management_system.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment1_hospital_management_system.SystemManager
{
    /// <summary>
    /// 統合されたメニューコントローラー - コード量を大幅削減
    /// </summary>
    public class MenuController
    {
        private DataManager dataManager;

        public MenuController(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }

        /// <summary>
        /// ユーザータイプに応じたメニュー表示とハンドリング
        /// </summary>
        public bool ShowUserMenu(User currentUser)
        {
            bool logout = false;
            bool exitSystem = false;

            while (!logout && !exitSystem)
            {
                currentUser.ShowMainMenu();

                try
                {
                    int choice = Utils.GetIntegerInput("Please enter your choice: ");
                    var result = HandleMenuChoice(currentUser, choice);
                    logout = result.logout;
                    exitSystem = result.exit;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Utils.PressAnyKeyToContinue();
                }
            }

            if (logout)
            {
                Console.WriteLine("Logged out successfully.");
                System.Threading.Thread.Sleep(1000);
            }

            return exitSystem;
        }

        /// <summary>
        /// 統一されたメニュー選択処理 - switch文で各ユーザータイプを処理
        /// </summary>
        private (bool logout, bool exit) HandleMenuChoice(User user, int choice)
        {
            // 共通のログアウト・終了処理
            var logoutExitOptions = GetLogoutExitOptions(user);
            if (choice == logoutExitOptions.logout) return (true, false);
            if (choice == logoutExitOptions.exit) return (false, true);

            // ユーザータイプ別の処理
            try
            {
                switch (user)
                {
                    case Patient patient:
                        HandlePatientMenu(patient, choice);
                        break;
                    case Doctor doctor:
                        HandleDoctorMenu(doctor, choice);
                        break;
                    case Administrator admin:
                        HandleAdminMenu(admin, choice);
                        break;
                    case Receptionist receptionist:
                        HandleReceptionistMenu(receptionist, choice);
                        break;
                    default:
                        Console.WriteLine("Unknown user type.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Utils.PressAnyKeyToContinue();
            }

            return (false, false);
        }

        /// <summary>
        /// returns the logout and exit option numbers based on user type
        /// </summary>
        private (int logout, int exit) GetLogoutExitOptions(User user) => user switch
        {
            Patient => (5, 6),
            Doctor => (6, 7),
            Administrator => (10, 11),
            Receptionist => (5, 6),
            _ => (99, 100)
        };

        #region Patient Menu Handlers
        private void HandlePatientMenu(Patient patient, int choice)
        {
            switch (choice)
            {
                case 1: ShowPatientDetails(patient); break;
                case 2: ShowMyDoctorDetails(patient); break;
                case 3: ShowPatientAppointments(patient); break;
                case 4: BookAppointment(patient); break;
                default: Console.WriteLine("Invalid option."); Utils.PressAnyKeyToContinue(); break;
            }
        }

        private void ShowPatientDetails(Patient patient)
        {
            Utils.DisplayHeader("My Details");
            Console.WriteLine($"Patient ID: {patient.Id}");
            Console.WriteLine($"Full Name: {patient.FirstName} {patient.LastName}");
            Console.WriteLine($"Address: {patient.Address}");
            Console.WriteLine($"Email: {patient.Email}");
            Console.WriteLine($"Phone: {patient.Phone}");
            Console.WriteLine($"Medical History: {patient.MedicalHistory}");
            Utils.PressAnyKeyToContinue();
        }

        private void ShowMyDoctorDetails(Patient patient)
        {
            Utils.DisplayHeader("My Doctor");
            if (patient.AssignedDoctorId.HasValue)
            {
                var doctor = dataManager.Doctors.FirstOrDefault(d => d.Id == patient.AssignedDoctorId.Value);
                if (doctor != null)
                {
                    Console.WriteLine($"Name: Dr. {doctor.FirstName} {doctor.LastName}");
                    Console.WriteLine($"Email: {doctor.Email}");
                    Console.WriteLine($"Phone: {doctor.Phone}");
                    Console.WriteLine($"Specialization: {doctor.Specialization}");
                }
                else Console.WriteLine("Doctor information not found.");
            }
            else Console.WriteLine("You are not currently assigned to a doctor.");
            Utils.PressAnyKeyToContinue();
        }

        private void ShowPatientAppointments(Patient patient)
        {
            Utils.DisplayHeader("My Appointments");
            var appointments = dataManager.Appointments.Where(a => a.PatientId == patient.Id).ToList();

            if (appointments.Any())
            {
                foreach (var appointment in appointments)
                {
                    var doctor = dataManager.Doctors.FirstOrDefault(d => d.Id == appointment.DoctorId);
                    string doctorName = doctor != null ? $"Dr. {doctor.FirstName} {doctor.LastName}" : "Unknown Doctor";
                    Console.WriteLine($"Doctor: {doctorName} | Description: {appointment.Description}");
                }
            }
            else Console.WriteLine("No appointments found.");
            Utils.PressAnyKeyToContinue();
        }

        private void BookAppointment(Patient patient)
        {
            Utils.DisplayHeader("Book Appointment");

            // 医師が割り当てられていない場合は選択させる
            if (!patient.AssignedDoctorId.HasValue)
            {
                Console.WriteLine("Choose a doctor to register with:");
                for (int i = 0; i < dataManager.Doctors.Count; i++)
                {
                    var doc = dataManager.Doctors[i];
                    Console.WriteLine($"{i + 1}. Dr. {doc.FirstName} {doc.LastName} | {doc.Specialization}");
                }

                int choice = Utils.GetIntegerInput("Enter doctor number: ") - 1;
                if (choice >= 0 && choice < dataManager.Doctors.Count)
                {
                    patient.AssignedDoctorId = dataManager.Doctors[choice].Id;
                    dataManager.Doctors[choice].AddPatient(patient.Id);
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                    Utils.PressAnyKeyToContinue();
                    return;
                }
            }

            string description = Utils.GetStringInput("Description: ");
            var appointment = new Appointment(patient.AssignedDoctorId.Value, patient.Id, description);
            dataManager.AddAppointment(appointment);

            Console.WriteLine("Appointment booked successfully!");
            Utils.PressAnyKeyToContinue();
        }
        #endregion

        #region Doctor Menu Handlers
        private void HandleDoctorMenu(Doctor doctor, int choice)
        {
            switch (choice)
            {
                case 1: ShowDoctorDetails(doctor); break;
                case 2: ShowDoctorPatients(doctor); break;
                case 3: ShowDoctorAppointments(doctor); break;
                case 4: CheckParticularPatient(); break;
                case 5: ListAppointmentsWithPatient(doctor); break;
                default: Console.WriteLine("Invalid option."); Utils.PressAnyKeyToContinue(); break;
            }
        }

        private void ShowDoctorDetails(Doctor doctor)
        {
            Utils.DisplayHeader("My Details");
            Console.WriteLine($"Name: Dr. {doctor.FirstName} {doctor.LastName}");
            Console.WriteLine($"Email: {doctor.Email}");
            Console.WriteLine($"Phone: {doctor.Phone}");
            Console.WriteLine($"Address: {doctor.Address}");
            Console.WriteLine($"Specialization: {doctor.Specialization}");
            Utils.PressAnyKeyToContinue();
        }

        private void ShowDoctorPatients(Doctor doctor)
        {
            Utils.DisplayHeader("My Patients");
            if (doctor.PatientIds.Any())
            {
                foreach (int patientId in doctor.PatientIds)
                {
                    var patient = dataManager.Patients.FirstOrDefault(p => p.Id == patientId);
                    if (patient != null)
                        Console.WriteLine($"Patient: {patient.FirstName} {patient.LastName} | Email: {patient.Email} | Phone: {patient.Phone}");
                }
            }
            else Console.WriteLine("No patients assigned.");
            Utils.PressAnyKeyToContinue();
        }

        private void ShowDoctorAppointments(Doctor doctor)
        {
            Utils.DisplayHeader("All Appointments");
            var appointments = dataManager.Appointments.Where(a => a.DoctorId == doctor.Id).ToList();

            if (appointments.Any())
            {
                foreach (var appointment in appointments)
                {
                    var patient = dataManager.Patients.FirstOrDefault(p => p.Id == appointment.PatientId);
                    string patientName = patient != null ? $"{patient.FirstName} {patient.LastName}" : "Unknown";
                    Console.WriteLine($"Patient: {patientName} | Description: {appointment.Description}");
                }
            }
            else Console.WriteLine("No appointments found.");
            Utils.PressAnyKeyToContinue();
        }

        private void CheckParticularPatient()
        {
            Utils.DisplayHeader("Check Patient Details");
            int patientId = Utils.GetIntegerInput("Enter patient ID: ");
            var patient = dataManager.Patients.FirstOrDefault(p => p.Id == patientId);

            if (patient != null)
                Console.WriteLine($"Patient: {patient.FirstName} {patient.LastName} | Email: {patient.Email} | Phone: {patient.Phone}");
            else
                Console.WriteLine($"No patient found with ID: {patientId}");
            Utils.PressAnyKeyToContinue();
        }

        private void ListAppointmentsWithPatient(Doctor doctor)
        {
            Utils.DisplayHeader("Appointments With Patient");
            int patientId = Utils.GetIntegerInput("Enter patient ID: ");
            var appointments = dataManager.Appointments.Where(a => a.DoctorId == doctor.Id && a.PatientId == patientId).ToList();

            if (appointments.Any())
            {
                var patient = dataManager.Patients.FirstOrDefault(p => p.Id == patientId);
                string patientName = patient?.FirstName + " " + patient?.LastName ?? "Unknown";
                foreach (var appointment in appointments)
                    Console.WriteLine($"Patient: {patientName} | Description: {appointment.Description}");
            }
            else Console.WriteLine($"No appointments found with patient ID: {patientId}");
            Utils.PressAnyKeyToContinue();
        }
        #endregion

        #region Admin Menu Handlers  
        private void HandleAdminMenu(Administrator admin, int choice)
        {
            switch (choice)
            {
                case 1: ShowAllDoctors(); break;
                case 2: CheckDoctorDetails(); break;
                case 3: ShowAllPatients(); break;
                case 4: CheckPatientDetails(); break;
                case 5: AddDoctor(); break;
                case 6: AddPatient(); break;
                case 7: AddReceptionist(); break;
                case 8: ShowFilteredPatients(); break;
                case 9: ShowSystemStatistics(); break;
                default: Console.WriteLine("Invalid option."); Utils.PressAnyKeyToContinue(); break;
            }
        }

        private void ShowAllDoctors()
        {
            Utils.DisplayHeader("All Doctors");
            Console.WriteLine("Name | Email | Phone | Address");
            Console.WriteLine("".PadRight(60, '-'));
            foreach (var doctor in dataManager.Doctors)
                Console.WriteLine($"Dr. {doctor.FirstName} {doctor.LastName} | {doctor.Email} | {doctor.Phone} | {doctor.Address}");
            Utils.PressAnyKeyToContinue();
        }

        private void CheckDoctorDetails()
        {
            Utils.DisplayHeader("Doctor Details");
            int doctorId = Utils.GetIntegerInput("Enter doctor ID: ");
            var doctor = dataManager.Doctors.FirstOrDefault(d => d.Id == doctorId);

            if (doctor != null)
                Console.WriteLine($"Dr. {doctor.FirstName} {doctor.LastName} | {doctor.Email} | {doctor.Phone} | {doctor.Address}");
            else
                Console.WriteLine($"No doctor found with ID: {doctorId}");
            Utils.PressAnyKeyToContinue();
        }

        private void ShowAllPatients()
        {
            Utils.DisplayHeader("All Patients");
            Console.WriteLine("Patient | Doctor | Email | Phone | Address");
            Console.WriteLine("".PadRight(70, '-'));

            foreach (var patient in dataManager.Patients)
            {
                var doctor = patient.AssignedDoctorId.HasValue ?
                    dataManager.Doctors.FirstOrDefault(d => d.Id == patient.AssignedDoctorId.Value) : null;
                string doctorName = doctor != null ? $"Dr. {doctor.FirstName} {doctor.LastName}" : "No Doctor";
                Console.WriteLine($"{patient.FirstName} {patient.LastName} | {doctorName} | {patient.Email} | {patient.Phone} | {patient.Address}");
            }
            Utils.PressAnyKeyToContinue();
        }

        private void CheckPatientDetails()
        {
            Utils.DisplayHeader("Patient Details");
            int patientId = Utils.GetIntegerInput("Enter patient ID: ");
            var patient = dataManager.Patients.FirstOrDefault(p => p.Id == patientId);

            if (patient != null)
            {
                var doctor = patient.AssignedDoctorId.HasValue ?
                    dataManager.Doctors.FirstOrDefault(d => d.Id == patient.AssignedDoctorId.Value) : null;
                string doctorName = doctor != null ? $"Dr. {doctor.FirstName} {doctor.LastName}" : "No Doctor";
                Console.WriteLine($"{patient.FirstName} {patient.LastName} | {doctorName} | {patient.Email} | {patient.Phone}");
            }
            else Console.WriteLine($"No patient found with ID: {patientId}");
            Utils.PressAnyKeyToContinue();
        }

        private void AddDoctor()
        {
            Utils.DisplayHeader("Add Doctor");
            var doctor = new Doctor(
                Utils.GetStringInput("First Name: "),
                Utils.GetStringInput("Last Name: "),
                Utils.GetStringInput("Specialization: ")
            )
            {
                Email = Utils.GetStringInput("Email: "),
                Phone = Utils.GetStringInput("Phone: "),
                Address = Utils.GetStringInput("Address: "),
                Password = "password123"
            };

            dataManager.AddDoctor(doctor);
            Console.WriteLine($"Dr {doctor.FirstName} {doctor.LastName} added! ID: {doctor.Id}");
            Utils.PressAnyKeyToContinue();
        }

        private void AddPatient()
        {
            Utils.DisplayHeader("Add Patient");
            var patient = new Patient(
                Utils.GetStringInput("First Name: "),
                Utils.GetStringInput("Last Name: ")
            )
            {
                Email = Utils.GetStringInput("Email: "),
                Phone = Utils.GetStringInput("Phone: "),
                Address = Utils.GetStringInput("Address: "),
                Password = "password123",
                MedicalHistory = "No significant medical history"
            };

            dataManager.AddPatient(patient);
            Console.WriteLine($"{patient.FirstName} {patient.LastName} added! ID: {patient.Id}");
            Utils.PressAnyKeyToContinue();
        }

        private void AddReceptionist()
        {
            Utils.DisplayHeader("Add Receptionist");
            var receptionist = new Receptionist(
                Utils.GetStringInput("First Name: "),
                Utils.GetStringInput("Last Name: ")
            )
            {
                Email = Utils.GetStringInput("Email: "),
                Phone = Utils.GetStringInput("Phone: "),
                Address = Utils.GetStringInput("Address: "),
                Password = "reception123"
            };

            dataManager.AddReceptionist(receptionist);
            Console.WriteLine($"{receptionist.FirstName} {receptionist.LastName} added! ID: {receptionist.Id}");
            Utils.PressAnyKeyToContinue();
        }

        private void ShowFilteredPatients()
        {
            Utils.DisplayHeader("Patient Filtering");
            Console.WriteLine("1. Patients with assigned doctors\n2. Patients with email\n3. All patients");

            int choice = Utils.GetIntegerInput("Select: ");
            var filteredPatients = choice switch
            {
                1 => dataManager.Patients.Where(p => p.AssignedDoctorId.HasValue).ToList(),
                2 => dataManager.Patients.Where(p => !string.IsNullOrEmpty(p.Email)).ToList(),
                3 => dataManager.Patients.ToList(),
                _ => new List<Patient>()
            };

            Console.WriteLine($"\nFilter Result: {filteredPatients.Count} patients");
            foreach (var patient in filteredPatients)
            {
                var doctor = dataManager.Doctors.FirstOrDefault(d => d.Id == patient.AssignedDoctorId);
                string doctorName = doctor != null ? $"Dr. {doctor.FirstName} {doctor.LastName}" : "Unassigned";
                Console.WriteLine($"{patient.FirstName} {patient.LastName} | Doctor: {doctorName} | Email: {patient.Email}");
            }
            Utils.PressAnyKeyToContinue();
        }

        private void ShowSystemStatistics()
        {
            Utils.DisplayHeader("System Statistics");
            Console.WriteLine($"Total Patients: {dataManager.Patients.Count}");
            Console.WriteLine($"Total Doctors: {dataManager.Doctors.Count}");
            Console.WriteLine($"Assigned Patients: {dataManager.Patients.Count(p => p.AssignedDoctorId.HasValue)}");
            Console.WriteLine($"Total Appointments: {dataManager.Appointments.Count}");
            Utils.PressAnyKeyToContinue();
        }
        #endregion

        #region Receptionist Menu Handlers
        private void HandleReceptionistMenu(Receptionist receptionist, int choice)
        {
            switch (choice)
            {
                case 1: RegisterNewPatient(receptionist); break;
                case 2: ViewExistingPatients(); break;
                case 3: ViewAppointments(); break;
                case 4: AddNewAppointment(receptionist); break;
                default: Console.WriteLine("Invalid option."); Utils.PressAnyKeyToContinue(); break;
            }
        }

        private void RegisterNewPatient(Receptionist receptionist)
        {
            Utils.DisplayHeader("Register New Patient");
            var patient = receptionist.RegisterNewPatient(
                Utils.GetStringInput("First Name: "),
                Utils.GetStringInput("Last Name: "),
                Utils.GetStringInput("Phone: "),
                Utils.GetStringInput("Email: "),
                Utils.GetStringInput("Address: ")
            );
            dataManager.AddPatient(patient);
            Console.WriteLine("Patient registration completed.");
            Utils.PressAnyKeyToContinue();
        }

        private void ViewExistingPatients()
        {
            Utils.DisplayHeader("View Existing Patients");
            if (!dataManager.Patients.Any())
            {
                Console.WriteLine("No registered patients found.");
                Utils.PressAnyKeyToContinue();
                return;
            }

            Console.WriteLine("ID | Name | Phone | Email");
            Console.WriteLine("".PadRight(60, '-'));
            foreach (var patient in dataManager.Patients)
                Console.WriteLine($"{patient.Id} | {patient.FirstName} {patient.LastName} | {patient.Phone} | {patient.Email}");
            Utils.PressAnyKeyToContinue();
        }

        private void ViewAppointments()
        {
            Utils.DisplayHeader("View Appointments");
            if (!dataManager.Appointments.Any())
            {
                Console.WriteLine("No appointments found.");
                Utils.PressAnyKeyToContinue();
                return;
            }

            Console.WriteLine("ID | Doctor | Patient | Description");
            Console.WriteLine("".PadRight(70, '-'));
            foreach (var appointment in dataManager.Appointments)
            {
                var doctor = dataManager.Doctors.FirstOrDefault(d => d.Id == appointment.DoctorId);
                var patient = dataManager.Patients.FirstOrDefault(p => p.Id == appointment.PatientId);
                Console.WriteLine($"{appointment.AppointmentId} | Dr. {doctor?.FirstName} {doctor?.LastName} | {patient?.FirstName} {patient?.LastName} | {appointment.Description}");
            }
            Utils.PressAnyKeyToContinue();
        }

        private void AddNewAppointment(Receptionist receptionist)
        {
            Utils.DisplayHeader("Add New Appointment");

            if (!dataManager.Patients.Any() || !dataManager.Doctors.Any())
            {
                Console.WriteLine("Insufficient data to create appointment.");
                Utils.PressAnyKeyToContinue();
                return;
            }

            // Patient selection
            Console.WriteLine("Select patient:");
            for (int i = 0; i < dataManager.Patients.Count; i++)
                Console.WriteLine($"{i + 1}. {dataManager.Patients[i].FirstName} {dataManager.Patients[i].LastName}");

            int patientChoice = Utils.GetIntegerInput("Patient number: ") - 1;
            if (patientChoice < 0 || patientChoice >= dataManager.Patients.Count) return;

            // Doctor selection
            Console.WriteLine("\nSelect doctor:");
            for (int i = 0; i < dataManager.Doctors.Count; i++)
                Console.WriteLine($"{i + 1}. Dr. {dataManager.Doctors[i].FirstName} {dataManager.Doctors[i].LastName}");

            int doctorChoice = Utils.GetIntegerInput("Doctor number: ") - 1;
            if (doctorChoice < 0 || doctorChoice >= dataManager.Doctors.Count) return;

            var appointment = receptionist.CreateAppointment(
                dataManager.Patients[patientChoice].Id,
                dataManager.Doctors[doctorChoice].Id,
                Utils.GetStringInput("Description: ")
            );

            dataManager.AddAppointment(appointment);
            Console.WriteLine("Appointment created successfully.");
            Utils.PressAnyKeyToContinue();
        }
        #endregion
    }
}