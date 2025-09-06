using Assignment1_hospital_management_system.Models;
using Assignment1_hospital_management_system.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment1_hospital_management_system.SystemManager
{
    /// <summary>
    /// Controls all menu operations and user interactions
    /// </summary>
    public class MenuController
    {
        private DataManager dataManager;
        private Dictionary<Type, IMenuHandler> menuHandlers;

        public MenuController(DataManager dataManager)
        {
            this.dataManager = dataManager;
            InitializeMenuHandlers();
        }

        private void InitializeMenuHandlers()
        {
            menuHandlers = new Dictionary<Type, IMenuHandler>
            {
                { typeof(Patient), new PatientMenuHandler(dataManager) },
                { typeof(Doctor), new DoctorMenuHandler(dataManager) },
                { typeof(Administrator), new AdminMenuHandler(dataManager) },
                { typeof(Receptionist), new ReceptionistMenuHandler(dataManager) }
            };
        }

        /// <summary>
        /// Show appropriate menu based on user type
        /// </summary>
        public bool ShowUserMenu(User currentUser)
        {
            var userType = currentUser.GetType();

            if (!menuHandlers.ContainsKey(userType))
            {
                Console.WriteLine($"No menu handler found for user type: {userType.Name}");
                return false;
            }

            var handler = menuHandlers[userType];
            bool logout = false;
            bool exitSystem = false;

            while (!logout && !exitSystem)
            {
                currentUser.ShowMainMenu();

                try
                {
                    int choice = Utils.GetIntegerInput("Please enter your choice: ");
                    var result = handler.HandleMenuChoice(currentUser, choice);
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
        /// デリゲートと匿名メソッドを使用した患者検索機能
        /// </summary>
        public void ShowFilteredPatients()
        {
            Utils.DisplayHeader("患者フィルタリング");

            Console.WriteLine("フィルタオプション:");
            Console.WriteLine("1. 医師が割り当て済みの患者");
            Console.WriteLine("2. メールアドレスが設定済みの患者");
            Console.WriteLine("3. 全ての患者");

            int choice = Utils.GetIntegerInput("選択してください: ");

            Delegates.UserFilter<Patient> filter = choice switch
            {
                1 => delegate (Patient p) { return p.AssignedDoctorId.HasValue; }
                ,
                2 => delegate (Patient p) { return !string.IsNullOrEmpty(p.Email); }
                ,
                3 => delegate (Patient p) { return true; }
                ,
                _ => null
            };

            if (filter == null)
            {
                Console.WriteLine("無効な選択です。");
                Utils.PressAnyKeyToContinue();
                return;
            }

            var filteredPatients = Delegates.FilterUsers(dataManager.Patients, filter);

            Delegates.LogAction logger = delegate (string message)
            {
                Console.WriteLine($"[ログ] {DateTime.Now:HH:mm:ss} - {message}");
            };

            Delegates.UserFormatter<Patient> formatter = delegate (Patient p)
            {
                var doctor = dataManager.Doctors.FirstOrDefault(d => d.Id == p.AssignedDoctorId);
                string doctorName = doctor != null ? $"Dr. {doctor.FirstName} {doctor.LastName}" : "未割り当て";
                return $"{p.FirstName} {p.LastName} | 担当医: {doctorName} | Email: {p.Email}";
            };

            Delegates.ExecuteWithLogging("患者一覧表示", logger, delegate ()
            {
                Console.WriteLine($"\nフィルタ結果: {filteredPatients.Count}人の患者");
                Console.WriteLine("".PadRight(70, '-'));
                Delegates.DisplayUsers(filteredPatients, formatter);
            });

            Utils.PressAnyKeyToContinue();
        }
    }

    /// <summary>
    /// 共通メニューハンドラーインターフェース
    /// </summary>
    public interface IMenuHandler
    {
        (bool logout, bool exit) HandleMenuChoice(User user, int choice);
    }

    /// <summary>
    /// 基底メニューハンドラークラス - 共通機能を統一
    /// </summary>
    public abstract class BaseMenuHandler : IMenuHandler
    {
        protected DataManager dataManager;

        // 各メニューの共通オプション設定
        protected abstract int LogoutOption { get; }
        protected abstract int ExitOption { get; }
        protected abstract Dictionary<int, MenuAction> MenuActions { get; }

        protected BaseMenuHandler(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }

        /// <summary>
        /// 統一されたメニュー選択処理
        /// </summary>
        public virtual (bool logout, bool exit) HandleMenuChoice(User user, int choice)
        {
            // 共通オプション（ログアウト/終了）の処理
            if (choice == LogoutOption)
            {
                return (true, false);
            }

            if (choice == ExitOption)
            {
                return (false, true);
            }

            // 各メニュー固有のアクション実行
            if (MenuActions.ContainsKey(choice))
            {
                try
                {
                    MenuActions[choice].Invoke(user);
                    return (false, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"メニュー処理中にエラーが発生しました: {ex.Message}");
                    Utils.PressAnyKeyToContinue();
                    return (false, false);
                }
            }

            // 無効な選択肢の処理
            Console.WriteLine("Invalid option. Please try again.");
            Utils.PressAnyKeyToContinue();
            return (false, false);
        }

        /// <summary>
        /// 共通メニューフッター表示
        /// </summary>
        protected void ShowMenuFooter()
        {
            Console.WriteLine("========================================");
        }

        /// <summary>
        /// 共通エラーハンドリング
        /// </summary>
        protected void HandleError(string operation, Exception ex)
        {
            Console.WriteLine($"Error in {operation}: {ex.Message}");
            Utils.PressAnyKeyToContinue();
        }
    }

    /// <summary>
    /// メニューアクションのデリゲート定義
    /// </summary>
    public delegate void MenuAction(User user);

    /// <summary>
    /// 患者メニューハンドラー
    /// </summary>
    public class PatientMenuHandler : BaseMenuHandler
    {
        protected override int LogoutOption => 5;
        protected override int ExitOption => 6;

        protected override Dictionary<int, MenuAction> MenuActions { get; }

        public PatientMenuHandler(DataManager dataManager) : base(dataManager)
        {
            MenuActions = new Dictionary<int, MenuAction>
            {
                { 1, user => ShowPatientDetails((Patient)user) },
                { 2, user => ShowMyDoctorDetails((Patient)user) },
                { 3, user => ShowPatientAppointments((Patient)user) },
                { 4, user => BookAppointment((Patient)user) }
            };
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
    /// 医師メニューハンドラー
    /// </summary>
    public class DoctorMenuHandler : BaseMenuHandler
    {
        protected override int LogoutOption => 6;
        protected override int ExitOption => 7;

        protected override Dictionary<int, MenuAction> MenuActions { get; }

        public DoctorMenuHandler(DataManager dataManager) : base(dataManager)
        {
            MenuActions = new Dictionary<int, MenuAction>
            {
                { 1, user => ShowDoctorDetails((Doctor)user) },
                { 2, user => ShowDoctorPatients((Doctor)user) },
                { 3, user => ShowDoctorAppointments((Doctor)user) },
                { 4, user => CheckParticularPatient((Doctor)user) },
                { 5, user => ListAppointmentsWithPatient((Doctor)user) }
            };
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
    /// 管理者メニューハンドラー
    /// </summary>
    public class AdminMenuHandler : BaseMenuHandler
    {
        protected override int LogoutOption => 10;
        protected override int ExitOption => 11;

        protected override Dictionary<int, MenuAction> MenuActions { get; }

        public AdminMenuHandler(DataManager dataManager) : base(dataManager)
        {
            MenuActions = new Dictionary<int, MenuAction>
            {
                { 1, user => ShowAllDoctors() },
                { 2, user => CheckDoctorDetails() },
                { 3, user => ShowAllPatients() },
                { 4, user => CheckPatientDetails() },
                { 5, user => AddDoctor() },
                { 6, user => AddPatient() },
                { 7, user => AddReceptionist() },
                { 8, user => ShowFilteredPatients() },
                { 9, user => ShowSystemStatisticsWithDelegates() }
            };
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

            int doctorId = Utils.GetIntegerInput("Please enter the ID of the doctor who's details you are checking: ");

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

            int patientId = Utils.GetIntegerInput("Please enter the ID of the patient who's details you are checking: ");

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
                Password = "password123"
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
                Password = "password123",
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

            Receptionist newReceptionist = new Receptionist(firstName, lastName)
            {
                Email = email,
                Phone = phone,
                Address = address,
                Password = "reception123"
            };

            dataManager.AddReceptionist(newReceptionist);

            Console.WriteLine();
            Console.WriteLine($"受付嬢 {firstName} {lastName} がシステムに追加されました！");
            Console.WriteLine($"受付嬢ID: {newReceptionist.Id}");
            Console.WriteLine($"デフォルトパスワード: reception123");

            Utils.PressAnyKeyToContinue();
        }

        private void ShowFilteredPatients()
        {
            Utils.DisplayHeader("患者フィルタリング");

            Console.WriteLine("フィルタオプション:");
            Console.WriteLine("1. 医師が割り当て済みの患者");
            Console.WriteLine("2. メールアドレスが設定済みの患者");
            Console.WriteLine("3. 全ての患者");

            int choice = Utils.GetIntegerInput("選択してください: ");

            Delegates.UserFilter<Patient> filter = choice switch
            {
                1 => delegate (Patient p) { return p.AssignedDoctorId.HasValue; }
                ,
                2 => delegate (Patient p) { return !string.IsNullOrEmpty(p.Email); }
                ,
                3 => delegate (Patient p) { return true; }
                ,
                _ => null
            };

            if (filter == null)
            {
                Console.WriteLine("無効な選択です。");
                Utils.PressAnyKeyToContinue();
                return;
            }

            var filteredPatients = Delegates.FilterUsers(dataManager.Patients, filter);

            Delegates.LogAction logger = delegate (string message)
            {
                Console.WriteLine($"[ログ] {DateTime.Now:HH:mm:ss} - {message}");
            };

            Delegates.UserFormatter<Patient> formatter = delegate (Patient p)
            {
                var doctor = dataManager.Doctors.FirstOrDefault(d => d.Id == p.AssignedDoctorId);
                string doctorName = doctor != null ? $"Dr. {doctor.FirstName} {doctor.LastName}" : "未割り当て";
                return $"{p.FirstName} {p.LastName} | 担当医: {doctorName} | Email: {p.Email}";
            };

            Delegates.ExecuteWithLogging("患者一覧表示", logger, delegate ()
            {
                Console.WriteLine($"\nフィルタ結果: {filteredPatients.Count}人の患者");
                Console.WriteLine("".PadRight(70, '-'));
                Delegates.DisplayUsers(filteredPatients, formatter);
            });

            Utils.PressAnyKeyToContinue();
        }

        private void ShowSystemStatisticsWithDelegates()
        {
            Utils.DisplayHeader("システム統計 (デリゲート使用)");

            Func<List<Patient>, int> patientCounter = delegate (List<Patient> patients)
            {
                return patients.Count;
            };

            Func<List<Doctor>, int> doctorCounter = delegate (List<Doctor> doctors)
            {
                return doctors.Count;
            };

            Func<List<Patient>, int> assignedPatientCounter = delegate (List<Patient> patients)
            {
                return patients.Count(p => p.AssignedDoctorId.HasValue);
            };

            Func<List<Patient>, int> unassignedPatientCounter = delegate (List<Patient> patients)
            {
                return patients.Count(p => !p.AssignedDoctorId.HasValue);
            };

            Action<string> systemLogger = delegate (string message)
            {
                Console.WriteLine($"[システム] {message}");
            };

            systemLogger("システム統計を計算中...");

            Console.WriteLine($"総患者数: {patientCounter(dataManager.Patients)}");
            Console.WriteLine($"総医師数: {doctorCounter(dataManager.Doctors)}");
            Console.WriteLine($"割り当て済み患者: {assignedPatientCounter(dataManager.Patients)}");
            Console.WriteLine($"未割り当て患者: {unassignedPatientCounter(dataManager.Patients)}");
            Console.WriteLine($"総予約数: {dataManager.Appointments.Count}");

            systemLogger("統計計算完了");
            Utils.PressAnyKeyToContinue();
        }
    }

    /// <summary>
    /// 受付嬢メニューハンドラー
    /// </summary>
    public class ReceptionistMenuHandler : BaseMenuHandler
    {
        protected override int LogoutOption => 5;
        protected override int ExitOption => 6;

        protected override Dictionary<int, MenuAction> MenuActions { get; }

        public ReceptionistMenuHandler(DataManager dataManager) : base(dataManager)
        {
            MenuActions = new Dictionary<int, MenuAction>
            {
                { 1, user => RegisterNewPatient((Receptionist)user) },
                { 2, user => ViewExistingPatients() },
                { 3, user => ViewAppointments() },
                { 4, user => AddNewAppointment((Receptionist)user) }
            };
        }

        private void RegisterNewPatient(Receptionist receptionist)
        {
            Utils.DisplayHeader("Register New Patient");

            string firstName = Utils.GetStringInput("患者の名前: ");
            string lastName = Utils.GetStringInput("患者の姓: ");
            string phone = Utils.GetStringInput("電話番号: ");
            string email = Utils.GetStringInput("メールアドレス: ");
            string address = Utils.GetStringInput("住所: ");

            Patient newPatient = receptionist.RegisterNewPatient(firstName, lastName, phone, email, address);
            dataManager.AddPatient(newPatient);

            Console.WriteLine("患者登録が完了しました。");
            Utils.PressAnyKeyToContinue();
        }

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