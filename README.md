# Hospital Management System

A console-based hospital management system built with C# for managing patients, doctors, administrators, and appointments.


## Login Credentials

On first run, the system creates default admin accounts with IDs.

**Default Passwords:**
- Administrator: `admin1234`
- Doctor: `doctor123` 
- Patient: `patient123`
- Receptionist: `reception123`

## User Roles

### Administrator
- View all doctors and patients
- Add new doctors, patients, and receptionists
- Check detailed user information

### Doctor
- View personal details and assigned patients
- Manage appointments
- Check specific patient information

### Patient
- View personal details and assigned doctor
- Book appointments
- View appointment history

### Receptionist
- Register new patients
- View existing patients and appointments
- Create appointments

## Data Storage

All data is stored in text files in the `Data` folder:
- `patients.txt`
- `doctors.txt`
- `administrators.txt`
- `appointments.txt`
- `receptionists.txt`

The system automatically creates these files on first run.

## Navigation

- Use number keys to select menu options
- Password input is masked with asterisks
- Press any key to return to menu after viewing information
- Use logout option to return to login screen
- Use exit option to close the application
