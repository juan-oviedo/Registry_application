# Registry

This application is a WPF-based employee management system designed to handle employee check-ins and check-outs, track late arrivals, and generate reports in Excel format. The system provides a user-friendly interface for administrators to manage employees and view or export attendance data.

## Features

- **Employee Management**: Add, delete, and manage employees in the system.
- **Check-In/Check-Out Tracking**: Employees can record their check-ins and check-outs. The system tracks late arrivals based on predefined schedules.
- **Reporting**: Generate Excel reports for employee attendance over a selected date range. The reports include details like late minutes and shift timings.
- **Settings Configuration**: Administrators can configure various system settings such as output directories, file names, and shift timings.
- **User Authentication**: Access to the settings and management pages is restricted through a simple login mechanism.
- **First-Time Setup**: The application includes a first-run setup process that guides new users through initial configuration steps.

## Technology Stack

- **C# / .NET**: The application is built using the .NET framework with WPF for the user interface.
- **SQLite**: Employee data and attendance records are stored in an SQLite database.
- **EPPlus**: Used for generating Excel reports from the attendance data.
- **WPF**: Provides the rich user interface and controls for the application.

## Getting Started

1. **Clone the Repository**: 
   ```bash
   git clone https://github.com/juan-oviedo/Registry_application
   ```
2. **Build the Application:**: 
- Open the solution in Visual Studio.
- Restore the NuGet packages.
- Build the solution.

3. **Run the Application:**: 
- Run the project in Visual Studio.
- On the first run, the application will prompt for initial setup.

## Usage

- Check-In/Check-Out: Navigate to the Check-In or Check-Out pages to record employee attendance.
- Generate Reports: Go to the Generate File page, select the date range, and generate an Excel report.
- Settings: Configure system settings like file paths, shift timings, and more in the Settings page.