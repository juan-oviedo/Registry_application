using Registro.Classes;
using Registro.Constants;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MessageBox = System.Windows.MessageBox;

namespace Registro.Pages
{
    /// <summary>
    /// Interaction logic for AddEmployee.xaml.
    /// Provides the functionality to add new employees to the system with roles and optional passwords.
    /// </summary>
    public partial class AddEmployee : Page
    {
        /// <summary>
        /// Indicates whether the current user has a developer role, which determines if the "Owner" role is available for selection.
        /// </summary>
        private bool isDevelop;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddEmployee"/> class.
        /// Sets up the UI elements based on the user's role.
        /// </summary>
        /// <param name="role">The role of the current user (optional). If the role is Developer, the "Owner" role option will be available.</param>
        public AddEmployee(string? role = null)
        {
            InitializeComponent();
            isDevelop = role == EmployeeRoles.Developer;
            PopulateRoleComboBox();
        }

        /// <summary>
        /// Populates the RoleComboBox with available roles based on the user's role.
        /// Developers can add Owners, while others can add Managers and Employees.
        /// </summary>
        private void PopulateRoleComboBox()
        {
            if (isDevelop)
            {
                ComboBoxItem ownerItem = new ComboBoxItem();
                ownerItem.Content = EmployeeRoles.Owner;
                RoleComboBox.Items.Add(ownerItem);
            }
            ComboBoxItem managerItem = new ComboBoxItem();
            managerItem.Content = EmployeeRoles.Manager;
            RoleComboBox.Items.Add(managerItem);

            ComboBoxItem employeeItem = new ComboBoxItem();
            employeeItem.Content = EmployeeRoles.Employee;
            RoleComboBox.Items.Add(employeeItem);
        }

        /// <summary>
        /// Handles the selection change event of the RoleComboBox.
        /// Displays or hides password fields based on the selected role.
        /// </summary>
        private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedRole = ((ComboBoxItem)RoleComboBox.SelectedItem)?.Content.ToString();
            if (selectedRole == EmployeeRoles.Manager || selectedRole == EmployeeRoles.Owner)
            {
                PasswordLabel.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Visible;
                ConfirmPasswordLabel.Visibility = Visibility.Visible;
                ConfirmPasswordTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                PasswordLabel.Visibility = Visibility.Collapsed;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                ConfirmPasswordLabel.Visibility = Visibility.Collapsed;
                ConfirmPasswordTextBox.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Handles the "Add Employee" button click event.
        /// Validates the input and inserts a new employee into the database.
        /// </summary>
        private void Btn_AddEmployee(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            string role = ((ComboBoxItem)RoleComboBox.SelectedItem)?.Content.ToString();
            string password = PasswordTextBox.Password;
            string confirmPassword = ConfirmPasswordTextBox.Password;

            // Validate role integrity
            if (role != EmployeeRoles.Employee && role != EmployeeRoles.Manager && role != EmployeeRoles.Owner)
            {
                throw new Exception("The role has been changed.");
            }

            // Validate input fields
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(role)
                || (role == EmployeeRoles.Manager || role == EmployeeRoles.Owner)
                        && (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword)))
            {
                MessageBox.Show("Por favor llena todos los campos.");
                return;
            }
            else if ((role == EmployeeRoles.Manager || role == EmployeeRoles.Owner) && password != confirmPassword)
            {
                MessageBox.Show("Las contraseñas no coinciden.");
                return;
            }

            // Attempt to insert the new employee into the database
            try
            {
                DatabaseHelper.InsertEmployee(name, role, password);
                MessageBox.Show($"Se añadio como {role} a {name} correctamente.");
                NameTextBox.Clear();
                PasswordTextBox.Clear();
                ConfirmPasswordTextBox.Clear();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show($"Intentalo de nuevo mas tarde.");
            }
        }

        /// <summary>
        /// Handles the "Go Back" button click event.
        /// Navigates back to the previous page if possible.
        /// </summary>
        private void Btn_GoBack(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}
