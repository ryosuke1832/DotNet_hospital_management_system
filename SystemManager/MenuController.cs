using Assignment1_hospital_management_system.Models;
using Assignment1_hospital_management_system.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1_hospital_management_system.SystemManager
{

    /// <summary>
    /// Controls all menu operations and user interactions
    /// </summary>
    public class MenuController
    {
        private DataManager dataManager;
        private PatientMenuHandler patientMenu;
        private DoctorMenuHandler doctorMenu;
        private AdminMenuHandler adminMenu;
        private ReceptionistMenuHandler receptionistMenu;

        public MenuController(DataManager dataManager)
        {
            this.dataManager = dataManager;
            this.patientMenu = new PatientMenuHandler(dataManager);
            this.doctorMenu = new DoctorMenuHandler(dataManager);
            this.adminMenu = new AdminMenuHandler(dataManager);
            this.receptionistMenu = new ReceptionistMenuHandler(dataManager);
        }

        /// <summary>
        /// Show appropriate menu based on user type
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

                    switch (currentUser.GetUserType())
                    {
                        case "Patient":
                            var result = patientMenu.HandleMenu((Patient)currentUser, choice);
                            logout = result.logout;
                            exitSystem = result.exit;
                            break;
                        case "Doctor":
                            var doctorResult = doctorMenu.HandleMenu((Doctor)currentUser, choice);
                            logout = doctorResult.logout;
                            exitSystem = doctorResult.exit;
                            break;
                        case "Administrator":
                            var adminResult = adminMenu.HandleMenu((Administrator)currentUser, choice);
                            logout = adminResult.logout;
                            exitSystem = adminResult.exit;
                            break;
                        case "Receptionist": 
                            var receptionistResult = receptionistMenu.HandleMenu((Receptionist)currentUser, choice);
                            logout = receptionistResult.logout;
                            exitSystem = receptionistResult.exit;
                            break;
                    }
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
    }

    /// <summary>
    /// Base class for menu handlers - provides common menu functionality
    /// </summary>
    public abstract class BaseMenuHandler
    {
        protected DataManager dataManager;

        protected BaseMenuHandler(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }

        /// <summary>
        /// Handle logout and exit options that are common to all menus
        /// </summary>
        protected (bool logout, bool exit) HandleCommonOptions(int choice, int logoutOption, int exitOption)
        {
            if (choice == logoutOption)
                return (true, false);
            if (choice == exitOption)
                return (false, true);

            Console.WriteLine("Invalid option. Please try again.");
            Utils.PressAnyKeyToContinue();
            return (false, false);
        }
    }

    /// <summary>
    /// Handles all patient menu operations
    /// </summary>
    public class PatientMenuHandler : BaseMenuHandler
    {
        public PatientMenuHandler(DataManager dataManager) : base(dataManager) { }

        /// <summary>
        /// Handle patient menu choices
        /// </summary>
        public (bool logout, bool exit) HandleMenu(Patient patient, int choice)
        {
            switch (choice)
            {
                case 1: // List patient details
                    ShowPatientDetails(patient);
                    return (false, false);
                case 2: // List my doctor details
                    ShowMyDoctorDetails(patient);
                    return (false, false);
                case 3: // List all appointments
                    ShowPatientAppointments(patient);
                    return (false, false);
                case 4: // Book appointment
                    BookAppointment(patient);
                    return (false, false);
                case 5: // Logout
                case 6: // Exit system
                    return HandleCommonOptions(choice, 5, 6);
                default:
                    return HandleCommonOptions(choice, 5, 6);
            }
        }

        private void ShowPatientDetails(Patient patient)
        {
            Utils.DisplayHeader("My Details");
            Console.WriteLine($"{patient.FirstName} {patient.LastName}'s Details");
            Console.WriteLine();
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
                Doctor doctor = dataManager.Doctors.FirstOrDefault(d => d.Id == patient.AssignedDoctorId.Value);
                if (doctor != null)
                {
                    Console.WriteLine("Your doctor:");
                    Console.WriteLine($"Name: Dr. {doctor.FirstName} {doctor.LastName}");
                    Console.WriteLine($"Email: {doctor.Email}");
                    Console.WriteLine($"Phone: {doctor.Phone}");
                    Console.WriteLine($"Specialization: {doctor.Specialization}");
                    Console.WriteLine($"Address: {doctor.Address}");
                }
                else
                {
                    Console.WriteLine("Doctor information not found.");
                }
            }
            else
            {
                Console.WriteLine("You are not currently assigned to a doctor.");
            }

            Utils.PressAnyKeyToContinue();
        }

        private void ShowPatientAppointments(Patient patient)
        {
            Utils.DisplayHeader("My Appointments");

            var patientAppointments = Utils.FindAppointmentsByPatientId(dataManager.Appointments, patient.Id);

            if (patientAppointments.Count > 0)
            {
                Console.WriteLine($"Appointments for {patient.FirstName} {patient.LastName}:");
                Console.WriteLine();
                foreach (var appointment in patientAppointments)
                {
                    var doctor = dataManager.Doctors.FirstOrDefault(d => d.Id == appointment.DoctorId);
                    string doctorName = doctor != null ? $"Dr. {doctor.FirstName} {doctor.LastName}" : "Unknown Doctor";
                    Console.WriteLine($"Doctor: {doctorName} | Patient: {patient.FirstName} {patient.LastName} | Description: {appointment.Description}");
                }
            }
            else
            {
                Console.WriteLine("No appointments found.");
            }

            Utils.PressAnyKeyToContinue();
        }

        private void BookAppointment(Patient patient)
        {
            Utils.DisplayHeader("Book Appointment");

            if (!patient.AssignedDoctorId.HasValue)
            {
                Console.WriteLine("You are not registered with a doctor. Please choose which doctor you would like to register with:");
                Console.WriteLine();

                for (int i = 0; i < dataManager.Doctors.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. Dr. {dataManager.Doctors[i].FirstName} {dataManager.Doctors[i].LastName} | Specialization: {dataManager.Doctors[i].Specialization}");
                }

                int doctorChoice = Utils.GetIntegerInput("Enter doctor number: ") - 1;

                if (doctorChoice >= 0 && doctorChoice < dataManager.Doctors.Count)
                {
                    patient.AssignedDoctorId = dataManager.Doctors[doctorChoice].Id;
                    dataManager.Doctors[doctorChoice].AddPatient(patient.Id);
                    Console.WriteLine($"You have been registered with Dr. {dataManager.Doctors[doctorChoice].FirstName} {dataManager.Doctors[doctorChoice].LastName}");
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                    Utils.PressAnyKeyToContinue();
                    return;
                }
            }

            string description = Utils.GetStringInput("Description of the appointment: ");

            Appointment newAppointment = new Appointment(patient.AssignedDoctorId.Value, patient.Id, description);
            dataManager.AddAppointment(newAppointment);

            var assignedDoctor = dataManager.Doctors.First(d => d.Id == patient.AssignedDoctorId);
            Console.WriteLine($"You are booking a new appointment with Dr. {assignedDoctor.FirstName} {assignedDoctor.LastName}");
            Console.WriteLine($"Description of the appointment: {description}");
            Console.WriteLine("The appointment has been booked successfully");

            Utils.PressAnyKeyToContinue();
        }
    }

    /// <summary>
    /// Handles all doctor menu operations
    /// </summary>
    public class DoctorMenuHandler : BaseMenuHandler
    {
        public DoctorMenuHandler(DataManager dataManager) : base(dataManager) { }

        /// <summary>
        /// Handle doctor menu choices
        /// </summary>
        public (bool logout, bool exit) HandleMenu(Doctor doctor, int choice)
        {
            switch (choice)
            {
                case 1: // List doctor details
                    ShowDoctorDetails(doctor);
                    return (false, false);
                case 2: // List patients
                    ShowDoctorPatients(doctor);
                    return (false, false);
                case 3: // List appointments
                    ShowDoctorAppointments(doctor);
                    return (false, false);
                case 4: // Check particular patient
                    CheckParticularPatient(doctor);
                    return (false, false);
                case 5: // List appointments with patient
                    ListAppointmentsWithPatient(doctor);
                    return (false, false);
                case 6: // Logout
                case 7: // Exit
                    return HandleCommonOptions(choice, 6, 7);
                default:
                    return HandleCommonOptions(choice, 6, 7);
            }
        }

        private void ShowDoctorDetails(Doctor doctor)
        {
            Utils.DisplayHeader("My Details");
            Console.WriteLine($"Name: Dr. {doctor.FirstName} {doctor.LastName}");
            Console.WriteLine($"Email Address: {doctor.Email}");
            Console.WriteLine($"Phone: {doctor.Phone}");
            Console.WriteLine($"Address: {doctor.Address}");
            Console.WriteLine($"Specialization: {doctor.Specialization}");
            Utils.PressAnyKeyToContinue();
        }

        private void ShowDoctorPatients(Doctor doctor)
        {
            Utils.DisplayHeader("My Patients");

            Console.WriteLine($"Patients assigned to Dr. {doctor.FirstName} {doctor.LastName}:");
            Console.WriteLine();

            if (doctor.PatientIds.Count > 0)
            {
                foreach (int patientId in doctor.PatientIds)
                {
                    Patient patient = dataManager.Patients.FirstOrDefault(p => p.Id == patientId);
                    if (patient != null)
                    {
                        Console.WriteLine($"Patient: {patient.FirstName} {patient.LastName} | Doctor: Dr. {doctor.FirstName} {doctor.LastName} | Email Address: {patient.Email} | Phone: {patient.Phone} | Address: {patient.Address}");
                    }
                }
            }
            else
            {
                Console.WriteLine("No patients assigned to this doctor.");
            }

            Utils.PressAnyKeyToContinue();
        }

        private void ShowDoctorAppointments(Doctor doctor)
        {
            Utils.DisplayHeader("All Appointments");

            var doctorAppointments = Utils.FindAppointmentsByDoctorId(dataManager.Appointments, doctor.Id);

            if (doctorAppointments.Count > 0)
            {
                Console.WriteLine("Doctor | Patient | Description");
                Console.WriteLine("".PadRight(50, '-'));

                foreach (var appointment in doctorAppointments)
                {
                    var patient = dataManager.Patients.FirstOrDefault(p => p.Id == appointment.PatientId);
                    string patientName = patient != null ? $"{patient.FirstName} {patient.LastName}" : "Unknown Patient";
                    Console.WriteLine($"Dr. {doctor.FirstName} {doctor.LastName} | {patientName} | {appointment.Description}");
                }
            }
            else
            {
                Console.WriteLine("No appointments found.");
            }

            Utils.PressAnyKeyToContinue();
        }

        private void CheckParticularPatient(Doctor doctor)
        {
            Utils.DisplayHeader("Check Patient Details");

            int patientId = Utils.GetIntegerInput("Enter the ID of the patient to check: ");

            Patient patient = dataManager.Patients.FirstOrDefault(p => p.Id == patientId);

            if (patient != null)
            {
                Console.WriteLine($"Patient | Doctor | Email Address | Phone | Address");
                Console.WriteLine("".PadRight(70, '-'));
                Console.WriteLine($"{patient.FirstName} {patient.LastName} | Dr. {doctor.FirstName} {doctor.LastName} | {patient.Email} | {patient.Phone} | {patient.Address}");
            }
            else
            {
                Console.WriteLine($"No patient found with ID: {patientId}");
            }

            Utils.PressAnyKeyToContinue();
        }

        private void ListAppointmentsWithPatient(Doctor doctor)
        {
            Utils.DisplayHeader("Appointments With");

            int patientId = Utils.GetIntegerInput("Enter the ID of the patient you would like to view appointments for: ");

            var appointmentsWithPatient = Utils.FindAppointmentsByDoctorAndPatient(dataManager.Appointments, doctor.Id, patientId);

            if (appointmentsWithPatient.Count > 0)
            {
                Patient patient = dataManager.Patients.FirstOrDefault(p => p.Id == patientId);
                string patientName = patient != null ? $"{patient.FirstName} {patient.LastName}" : "Unknown Patient";

                Console.WriteLine("Doctor | Patient | Description");
                Console.WriteLine("".PadRight(50, '-'));

                foreach (var appointment in appointmentsWithPatient)
                {
                    Console.WriteLine($"Dr. {doctor.FirstName} {doctor.LastName} | {patientName} | {appointment.Description}");
                }
            }
            else
            {
                Console.WriteLine($"No appointments found with patient ID: {patientId}");
            }

            Utils.PressAnyKeyToContinue();
        }
    }

    /// <summary>
    /// Handles all administrator menu operations
    /// </summary>
    public class AdminMenuHandler : BaseMenuHandler
    {
        public AdminMenuHandler(DataManager dataManager) : base(dataManager) { }

        /// <summary>
        /// Handle administrator menu choices
        /// </summary>
        public (bool logout, bool exit) HandleMenu(Administrator admin, int choice)
        {
            switch (choice)
            {
                case 1: // List all doctors
                    ShowAllDoctors();
                    return (false, false);
                case 2: // Check doctor details
                    CheckDoctorDetails();
                    return (false, false);
                case 3: // List all patients
                    ShowAllPatients();
                    return (false, false);
                case 4: // Check patient details
                    CheckPatientDetails();
                    return (false, false);
                case 5: // Add doctor
                    AddDoctor();
                    return (false, false);
                case 6: // Add patient
                    AddPatient();
                    return (false, false);
                case 7: // Add receptionist - 新しく追加
                    AddReceptionist();
                    return (false, false);
                case 8: // Logout
                case 9: // Exit
                    return HandleCommonOptions(choice, 7, 8);
                default:
                    return HandleCommonOptions(choice, 7, 8);
            }
        }

        private void ShowAllDoctors()
        {
            Utils.DisplayHeader("All Doctors");

            Console.WriteLine("All doctors registered to the DOTNET Hospital Management System");
            Console.WriteLine();
            Console.WriteLine("Name | Email Address | Phone | Address");
            Console.WriteLine("".PadRight(60, '-'));

            foreach (Doctor doctor in dataManager.Doctors)
            {
                Console.WriteLine($"Dr. {doctor.FirstName} {doctor.LastName} | {doctor.Email} | {doctor.Phone} | {doctor.Address}");
            }

            Utils.PressAnyKeyToContinue();
        }

        private void CheckDoctorDetails()
        {
            Utils.DisplayHeader("Doctor Details");

            int doctorId = Utils.GetIntegerInput("Please enter the ID of the doctor who's details you are checking, or press n to return to menu: ");

            Doctor doctor = dataManager.Doctors.FirstOrDefault(d => d.Id == doctorId);

            if (doctor != null)
            {
                Console.WriteLine($"Details for Dr. {doctor.FirstName} {doctor.LastName}");
                Console.WriteLine();
                Console.WriteLine("Name | Email Address | Phone | Address");
                Console.WriteLine("".PadRight(60, '-'));
                Console.WriteLine($"Dr. {doctor.FirstName} {doctor.LastName} | {doctor.Email} | {doctor.Phone} | {doctor.Address}");
            }
            else
            {
                Console.WriteLine($"No doctor found with ID: {doctorId}");
            }

            Utils.PressAnyKeyToContinue();
        }

        private void ShowAllPatients()
        {
            Utils.DisplayHeader("All Patients");

            Console.WriteLine("All patients registered to the DOTNET Hospital Management System");
            Console.WriteLine();
            Console.WriteLine("Patient | Doctor | Email Address | Phone | Address");
            Console.WriteLine("".PadRight(70, '-'));

            foreach (Patient patient in dataManager.Patients)
            {
                Doctor assignedDoctor = null;
                if (patient.AssignedDoctorId.HasValue)
                {
                    assignedDoctor = dataManager.Doctors.FirstOrDefault(d => d.Id == patient.AssignedDoctorId.Value);
                }

                string doctorName = assignedDoctor != null ? $"Dr. {assignedDoctor.FirstName} {assignedDoctor.LastName}" : "No Doctor Assigned";
                Console.WriteLine($"{patient.FirstName} {patient.LastName} | {doctorName} | {patient.Email} | {patient.Phone} | {patient.Address}");
            }

            Utils.PressAnyKeyToContinue();
        }

        private void CheckPatientDetails()
        {
            Utils.DisplayHeader("Patient Details");

            int patientId = Utils.GetIntegerInput("Please enter the ID of the patient who's details you are checking, or press n to return to menu: ");

            Patient patient = dataManager.Patients.FirstOrDefault(p => p.Id == patientId);

            if (patient != null)
            {
                Doctor assignedDoctor = null;
                if (patient.AssignedDoctorId.HasValue)
                {
                    assignedDoctor = dataManager.Doctors.FirstOrDefault(d => d.Id == patient.AssignedDoctorId.Value);
                }

                Console.WriteLine($"Details for {patient.FirstName} {patient.LastName}");
                Console.WriteLine();
                Console.WriteLine("Patient | Doctor | Email Address | Phone | Address");
                Console.WriteLine("".PadRight(70, '-'));

                string doctorName = assignedDoctor != null ? $"Dr. {assignedDoctor.FirstName} {assignedDoctor.LastName}" : "No Doctor Assigned";
                Console.WriteLine($"{patient.FirstName} {patient.LastName} | {doctorName} | {patient.Email} | {patient.Phone} | {patient.Address}");
            }
            else
            {
                Console.WriteLine($"No patient found with ID: {patientId}");
            }

            Utils.PressAnyKeyToContinue();
        }

        private void AddDoctor()
        {
            Utils.DisplayHeader("Add Doctor");

            Console.WriteLine("Registering a new doctor with the DOTNET Hospital Management System");

            string firstName = Utils.GetStringInput("First Name: ");
            string lastName = Utils.GetStringInput("Last Name: ");
            string email = Utils.GetStringInput("Email: ");
            string phone = Utils.GetStringInput("Phone: ");
            string address = Utils.GetStringInput("Address: ");
            string specialization = Utils.GetStringInput("Specialization: ");

            Doctor newDoctor = new Doctor(firstName, lastName, specialization)
            {
                Email = email,
                Phone = phone,
                Address = address,
                Password = "password123" // Default password
            };

            dataManager.AddDoctor(newDoctor);

            Console.WriteLine($"Dr {firstName} {lastName} added to the system!");
            Console.WriteLine($"Doctor ID: {newDoctor.Id}");
            Console.WriteLine($"Default Password: password123");

            Utils.PressAnyKeyToContinue();
        }

        private void AddPatient()
        {
            Utils.DisplayHeader("Add Patient");

            Console.WriteLine("Registering a new patient with the DOTNET Hospital Management System");

            string firstName = Utils.GetStringInput("First Name: ");
            string lastName = Utils.GetStringInput("Last Name: ");
            string email = Utils.GetStringInput("Email: ");
            string phone = Utils.GetStringInput("Phone: ");
            string address = Utils.GetStringInput("Address: ");

            Patient newPatient = new Patient(firstName, lastName)
            {
                Email = email,
                Phone = phone,
                Address = address,
                Password = "password123", // Default password
                MedicalHistory = "No significant medical history"
            };

            dataManager.AddPatient(newPatient);

            Console.WriteLine($"{firstName} {lastName} added to the system!");
            Console.WriteLine($"Patient ID: {newPatient.Id}");
            Console.WriteLine($"Default Password: password123");

            Utils.PressAnyKeyToContinue();
        }
        private void AddReceptionist()
        {
            Utils.DisplayHeader("Add Receptionist");

            Console.WriteLine("新しい受付嬢をシステムに登録します");
            Console.WriteLine();

            string firstName = Utils.GetStringInput("名前: ");
            string lastName = Utils.GetStringInput("姓: ");
            string email = Utils.GetStringInput("メールアドレス: ");
            string phone = Utils.GetStringInput("電話番号: ");
            string address = Utils.GetStringInput("住所: ");

            // 新しい受付嬢を作成
            Receptionist newReceptionist = new Receptionist(firstName, lastName)
            {
                Email = email,
                Phone = phone,
                Address = address,
                Password = "reception123" // デフォルトパスワード
            };

            // システムに追加
            dataManager.AddReceptionist(newReceptionist);

            Console.WriteLine();
            Console.WriteLine($"受付嬢 {firstName} {lastName} がシステムに追加されました！");
            Console.WriteLine($"受付嬢ID: {newReceptionist.Id}");
            Console.WriteLine($"デフォルトパスワード: reception123");

            Utils.PressAnyKeyToContinue();
        }

    }

    /// <summary>
    /// 受付嬢メニューハンドラークラス（最小限機能）
    /// </summary>
    public class ReceptionistMenuHandler : BaseMenuHandler
    {
        public ReceptionistMenuHandler(DataManager dataManager) : base(dataManager) { }

        /// <summary>
        /// 受付嬢メニューの選択処理
        /// </summary>
        public (bool logout, bool exit) HandleMenu(Receptionist receptionist, int choice)
        {
            switch (choice)
            {
                case 1: // Register new patient
                    RegisterNewPatient(receptionist);
                    return (false, false);
                case 2: // View existing patients
                    ViewExistingPatients();
                    return (false, false);
                case 3: // View appointments
                    ViewAppointments();
                    return (false, false);
                case 4: // Add new appointment
                    AddNewAppointment(receptionist);
                    return (false, false);
                case 5: // Logout
                case 6: // Exit
                    return HandleCommonOptions(choice, 5, 6);
                default:
                    return HandleCommonOptions(choice, 5, 6);
            }
        }

        /// <summary>
        /// 機能1: 新規患者登録
        /// </summary>
        private void RegisterNewPatient(Receptionist receptionist)
        {
            Utils.DisplayHeader("Register New Patient");

            string firstName = Utils.GetStringInput("患者の名前: ");
            string lastName = Utils.GetStringInput("患者の姓: ");
            string phone = Utils.GetStringInput("電話番号: ");
            string email = Utils.GetStringInput("メールアドレス: ");
            string address = Utils.GetStringInput("住所: ");

            // 受付嬢のメソッドを使用して患者を登録
            Patient newPatient = receptionist.RegisterNewPatient(firstName, lastName, phone, email, address);

            // システムに患者を追加
            dataManager.AddPatient(newPatient);

            Console.WriteLine("患者登録が完了しました。");
            Utils.PressAnyKeyToContinue();
        }

        /// <summary>
        /// 機能2: 既存患者の閲覧
        /// </summary>
        private void ViewExistingPatients()
        {
            Utils.DisplayHeader("View Existing Patients");

            if (dataManager.Patients.Count == 0)
            {
                Console.WriteLine("登録されている患者はいません。");
                Utils.PressAnyKeyToContinue();
                return;
            }

            Console.WriteLine($"登録患者一覧 (合計: {dataManager.Patients.Count}名)");
            Console.WriteLine();
            Console.WriteLine("ID | 名前 | 電話番号 | メールアドレス");
            Console.WriteLine("".PadRight(60, '-'));

            foreach (var patient in dataManager.Patients)
            {
                Console.WriteLine($"{patient.Id} | {patient.FirstName} {patient.LastName} | {patient.Phone} | {patient.Email}");
            }

            Utils.PressAnyKeyToContinue();
        }

        /// <summary>
        /// 機能3: 予約一覧表示
        /// </summary>
        private void ViewAppointments()
        {
            Utils.DisplayHeader("View Appointments");

            if (dataManager.Appointments.Count == 0)
            {
                Console.WriteLine("登録されている予約はありません。");
                Utils.PressAnyKeyToContinue();
                return;
            }

            Console.WriteLine($"予約一覧 (合計: {dataManager.Appointments.Count}件)");
            Console.WriteLine();
            Console.WriteLine("予約ID | 医師 | 患者 | 内容");
            Console.WriteLine("".PadRight(70, '-'));

            foreach (var appointment in dataManager.Appointments)
            {
                var doctor = dataManager.Doctors.FirstOrDefault(d => d.Id == appointment.DoctorId);
                var patient = dataManager.Patients.FirstOrDefault(p => p.Id == appointment.PatientId);

                string doctorName = doctor != null ? $"Dr. {doctor.FirstName} {doctor.LastName}" : "Unknown";
                string patientName = patient != null ? $"{patient.FirstName} {patient.LastName}" : "Unknown";

                Console.WriteLine($"{appointment.AppointmentId} | {doctorName} | {patientName} | {appointment.Description}");
            }

            Utils.PressAnyKeyToContinue();
        }

        /// <summary>
        /// 機能4: 予約の追加
        /// </summary>
        private void AddNewAppointment(Receptionist receptionist)
        {
            Utils.DisplayHeader("Add New Appointment");

            if (dataManager.Patients.Count == 0)
            {
                Console.WriteLine("患者が登録されていません。");
                Utils.PressAnyKeyToContinue();
                return;
            }

            if (dataManager.Doctors.Count == 0)
            {
                Console.WriteLine("医師が登録されていません。");
                Utils.PressAnyKeyToContinue();
                return;
            }

            // 患者選択
            Console.WriteLine("患者を選択してください:");
            for (int i = 0; i < dataManager.Patients.Count; i++)
            {
                var patient = dataManager.Patients[i];
                Console.WriteLine($"{i + 1}. {patient.FirstName} {patient.LastName} (ID: {patient.Id})");
            }

            int patientChoice = Utils.GetIntegerInput("患者番号: ") - 1;
            if (patientChoice < 0 || patientChoice >= dataManager.Patients.Count)
            {
                Console.WriteLine("無効な選択です。");
                Utils.PressAnyKeyToContinue();
                return;
            }

            Patient selectedPatient = dataManager.Patients[patientChoice];

            // 医師選択
            Console.WriteLine();
            Console.WriteLine("医師を選択してください:");
            for (int i = 0; i < dataManager.Doctors.Count; i++)
            {
                var doctor = dataManager.Doctors[i];
                Console.WriteLine($"{i + 1}. Dr. {doctor.FirstName} {doctor.LastName} - {doctor.Specialization}");
            }

            int doctorChoice = Utils.GetIntegerInput("医師番号: ") - 1;
            if (doctorChoice < 0 || doctorChoice >= dataManager.Doctors.Count)
            {
                Console.WriteLine("無効な選択です。");
                Utils.PressAnyKeyToContinue();
                return;
            }

            Doctor selectedDoctor = dataManager.Doctors[doctorChoice];

            // 予約内容入力
            string description = Utils.GetStringInput("予約内容: ");

            // 受付嬢のメソッドを使用して予約を作成
            Appointment newAppointment = receptionist.CreateAppointment(selectedPatient.Id, selectedDoctor.Id, description);

            // システムに予約を追加
            dataManager.AddAppointment(newAppointment);

            // 患者に医師を割り当て（まだ割り当てられていない場合）
            if (!selectedPatient.AssignedDoctorId.HasValue)
            {
                selectedPatient.AssignedDoctorId = selectedDoctor.Id;
                selectedDoctor.AddPatient(selectedPatient.Id);
            }

            Console.WriteLine("予約が正常に作成されました。");
            Utils.PressAnyKeyToContinue();
        }
    }

}
