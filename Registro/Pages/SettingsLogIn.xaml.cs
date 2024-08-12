using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MessageBox = System.Windows.MessageBox;

namespace Registro.Pages
{
    /// <summary>
    /// Interaction logic for SettingsLogIn.xaml.
    /// Provides the login interface for accessing the settings in the application.
    /// </summary>
    public partial class SettingsLogIn : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsLogIn"/> class.
        /// Sets up the UI components.
        /// </summary>
        public SettingsLogIn()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the "Login" button click event.
        /// Validates the entered password and, if correct, navigates to the settings main page.
        /// Displays an error message if the credentials are incorrect or if fields are empty.
        /// </summary>
        private void Btn_Login(object sender, RoutedEventArgs e)
        {
            string password = PasswordTextBox.Password;


            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor, llena todos los campos.");
                return;
            }

            // Validate the password
            if (password == "3517718382")
            {
                MessageBox.Show("Inicio de sesión exitoso.");
                NavigationService.Navigate(new SettingsMain());
            }
            else
            {
                MessageBox.Show("Credenciales incorrectas. Inténtalo de nuevo.");
            }
        }
    }
}
