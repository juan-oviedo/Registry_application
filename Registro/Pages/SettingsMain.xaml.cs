using Registro.Constants;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Registro.Pages
{
    /// <summary>
    /// Interaction logic for SettingsMain.xaml.
    /// Provides the main interface for managing settings, including options to add or delete employees and navigate to other settings.
    /// </summary>
    public partial class SettingsMain : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsMain"/> class.
        /// Sets up the UI components.
        /// </summary>
        public SettingsMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the "Add Employee" button click event.
        /// Navigates to the AddEmployee page with the Developer role specified, allowing the user to add new employees.
        /// </summary>
        private void Btn_AddEmployee(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEmployee(EmployeeRoles.Developer));
        }

        /// <summary>
        /// Handles the "Delete Employee" button click event.
        /// Navigates to the DeleteEmployee page with the Developer role specified, allowing the user to delete employees.
        /// </summary>
        private void Btn_DeleteEmployee(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DeleteEmployee(EmployeeRoles.Developer));
        }

        /// <summary>
        /// Handles the "Settings" button click event.
        /// Navigates to the general Settings page.
        /// </summary>
        private void Btn_Settings(object sender, RoutedEventArgs e)
        {
            // Navigate to Settings page
            NavigationService.Navigate(new Settings());
        }

        /// <summary>
        /// Handles the "Close" button click event.
        /// Closes the current window, returning the user to the previous interface or shutting down the settings interface.
        /// </summary>
        private void Btn_Close(object sender, RoutedEventArgs e)
        {
            Window currentWindow = Window.GetWindow(this);
            currentWindow.Close();
        }
    }
}
