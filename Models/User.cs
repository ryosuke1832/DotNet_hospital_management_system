
using Assignment1_hospital_management_system.SystemManager;

namespace Assignment1_hospital_management_system.Models
{
    public abstract class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }

        // DataManager reference for unique ID generation
        private static DataManager _dataManager;

        public static void SetDataManager(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        // Constructor base - Generate unique ID using DataManager
        public User()
        {
            if (_dataManager != null)
            {
                Id = _dataManager.GenerateUniqueId(); 
            }
            else
            {
                throw new InvalidOperationException(
                    "DataManager must be set before creating User objects. Call User.SetDataManager() first.");
            }
        }

        public User(string firstName, string lastName) : this()
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public User(string firstName, string lastName, string email, string phone) : this(firstName, lastName)
        {
            Email = email;
            Phone = phone;
        }


        

        public abstract void ShowMainMenu();
        public abstract string GetUserType();

        public override string ToString()
        {
            return $"{FirstName} {LastName} (ID: {Id})";
        }

        public bool Login(int id, string password)
        {
            return Id == id && Password == password;
        }

        public void SetId(int id)
        {
            Id = id;
        }
    }
}
