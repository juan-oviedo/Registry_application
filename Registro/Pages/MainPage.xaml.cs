using Registro.Constants;
using Registro.Windows;
using System.Windows;
using System.Windows.Controls;
using Application = System.Windows.Application;
using Image = System.Windows.Controls.Image;

namespace Registro.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml.
    /// Represents the main interface of the application, allowing navigation to different sections based on user role.
    /// </summary>
    public partial class MainPage : Page
    {
        /// <summary>
        /// The name of the logged-in user.
        /// </summary>
        private string userName;

        /// <summary>
        /// The role of the logged-in user.
        /// </summary>
        private string userRole;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// Sets up the UI based on the user's role.
        /// </summary>
        /// <param name="name">The name of the logged-in user.</param>
        /// <param name="role">The role of the logged-in user.</param>
        public MainPage(string name, string role)
        {
            InitializeComponent();
            userName = name;
            userRole = role;
            if (role == EmployeeRoles.Manager)
            {
                AddEmployeeItem.Visibility = Visibility.Collapsed;
                DeleteEmployeeItem.Visibility = Visibility.Collapsed;
                SettingsBtn.Visibility = Visibility.Collapsed;
            }
            UserInfoTextBlock.Text = $"Usuario: {userName}\nRol: {userRole}";

        }

        /// <summary>
        /// Handles the "Add Employee" button click event.
        /// Navigates to the AddEmployee page.
        /// </summary>
        private void Btn_AddEmployee(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEmployee());
        }

        /// <summary>
        /// Handles the "Delete Employee" button click event.
        /// Navigates to the DeleteEmployee page.
        /// </summary>
        private void Btn_DeleteEmployee(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DeleteEmployee());
        }

        /// <summary>
        /// Handles the "Check In" button click event.
        /// Navigates to the CheckIn page.
        /// </summary>
        private void Btn_CheckIn(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CheckIn());
        }

        /// <summary>
        /// Handles the "Check Out" button click event.
        /// Navigates to the CheckOut page.
        /// </summary>
        private void Btn_Checkout(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CheckOut());
        }

        /// <summary>
        /// Handles the "Generate File" button click event.
        /// Navigates to the GenerateFile page.
        /// </summary>
        private void Btn_Generar(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GenerateFile());
        }

        /// <summary>
        /// Handles the "Settings" button click event.
        /// Navigates to the Settings page.
        /// </summary>
        private void Btn_Settings(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Settings());
        }

        /// <summary>
        /// Handles the "Exit" button click event.
        /// Closes the application.
        /// </summary>
        private void Btn_Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Handles the settings icon click event.
        /// Displays a context menu with additional options.
        /// </summary>
        private void SettingsIcon_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Show the context menu with the options
            var image = sender as Image;
            if (image != null)
            {
                image.ContextMenu.IsOpen = true;
            }
        }

        /// <summary>
        /// Handles the "Enter as Developer" menu item click event.
        /// Opens the FirstRunWindow for developers.
        /// </summary>
        private void EnterAsDeveloperMenuItem_Click(object sender, RoutedEventArgs e)
        {
            FirstRunWindow firstRunWindow = new FirstRunWindow();
            firstRunWindow.Show();
        }
    }
}
