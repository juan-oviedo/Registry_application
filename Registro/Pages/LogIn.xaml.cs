using Registro.Classes;
using Registro.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MessageBox = System.Windows.MessageBox;

namespace Registro.Pages
{
    /// <summary>
    /// Interaction logic for LogIn.xaml.
    /// Represents the login page where users can enter their credentials to access the system.
    /// </summary>
    public partial class LogIn : Page
    {
        /// <summary>
        /// List of all employees with their IDs, names, roles, and hashed passwords.
        /// </summary>
        List<(int Id, string Name, string Rol, string Password)> employees;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogIn"/> class.
        /// Retrieves all employees' credentials from the database.
        /// </summary>
        public LogIn()
        {
            InitializeComponent();
            employees = DatabaseHelper.GetAllEmployeesPasswords();
        }

        /// <summary>
        /// Handles the login button click event.
        /// Validates the user's password and navigates to the main page if successful.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        private void Btn_Login(object sender, RoutedEventArgs e)
        {
            string password = PasswordTextBox.Password;


            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor, llena todos los campos.");
                return;
            }

            string hashPassword = PasswordHelper.HashPassword(password);

            var matchingEmployee = employees.FirstOrDefault(emp => emp.Password == hashPassword);

            if (matchingEmployee != default)
            {
                // Store the logged-in employee's details
                int logId = matchingEmployee.Id;
                string logName = matchingEmployee.Name;
                string logRol = matchingEmployee.Rol;

                CheckHelper.CheckInForOpening(logId);

                MessageBox.Show("Inicio de sesión exitoso.");
                NavigationService.Navigate(new MainPage(logName, logRol));
            }
            else
            {
                MessageBox.Show("Credenciales incorrectas. Inténtalo de nuevo.");
            }
        }

        /// <summary>
        /// Handles the click event on the settings icon.
        /// Displays a context menu with an option to enter as a developer.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        private void SettingsIcon_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Show a context menu with the options
            ContextMenu contextMenu = new ContextMenu();
            MenuItem enterAsDeveloperMenuItem = new MenuItem { Header = "Entrar como Desarrollador" };

            enterAsDeveloperMenuItem.Click += EnterAsDeveloperMenuItem_Click;

            contextMenu.Items.Add(enterAsDeveloperMenuItem);

            contextMenu.IsOpen = true;
        }

        /// <summary>
        /// Handles the click event for the "Enter as Developer" menu item.
        /// Opens the FirstRunWindow for developers.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        private void EnterAsDeveloperMenuItem_Click(object sender, RoutedEventArgs e)
        {
            FirstRunWindow firstRunWindow = new FirstRunWindow();
            firstRunWindow.Show();
        }
    }
}
