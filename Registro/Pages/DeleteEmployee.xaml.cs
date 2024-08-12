using Registro.Classes;
using Registro.Constants;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MessageBox = System.Windows.MessageBox;

namespace Registro.Pages
{
    /// <summary>
    /// Interaction logic for DeleteEmployee.xaml.
    /// Provides the functionality to delete employees from the system, displaying a list of non-deleted employees for selection.
    /// </summary>
    public partial class DeleteEmployee : Page
    {
        /// <summary>
        /// Indicates whether the current user has a developer role, which influences the list of employees that can be deleted.
        /// </summary>
        private bool isDevelop;

        /// <summary>
        /// A list of tuples containing the ID, name, and position of employees that are eligible for deletion.
        /// </summary>
        private List<(int Id, string Name, string Position)> employees;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteEmployee"/> class.
        /// Sets up the UI elements and loads the list of employees based on the user's role.
        /// </summary>
        /// <param name="role">The role of the current user (optional). If the role is Developer, a different set of employees might be loaded.</param>
        public DeleteEmployee(string? role = null)
        {
            InitializeComponent();
            isDevelop = role == EmployeeRoles.Developer;
            LoadEmployees();
        }

        /// <summary>
        /// Loads the list of non-deleted employees from the database and populates the ComboBox for selection.
        /// The list may vary depending on whether the user is a developer.
        /// </summary>
        private void LoadEmployees()
        {
            employees = DatabaseHelper.GetNonDeletedEmployeesWithRoles(isDevelop);
            EmployeeComboBox.ItemsSource = employees.Select(e => e.Name).ToList();
        }

        /// <summary>
        /// Handles the selection change event of the EmployeeComboBox.
        /// Displays the selected employee's role in the UI.
        /// </summary>
        private void EmployeeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeeComboBox.SelectedIndex >= 0)
            {
                var selectedEmployee = employees[EmployeeComboBox.SelectedIndex];
                RoleTextBlock.Text = $"{selectedEmployee.Position}";
            }
            else
            {
                RoleTextBlock.Text = string.Empty;
            }
        }

        /// <summary>
        /// Handles the "Delete Employee" button click event.
        /// Deletes the selected employee from the database and reloads the employee list.
        /// </summary>
        private void Btn_DeleteEmployee(object sender, RoutedEventArgs e)
        {
            if (EmployeeComboBox.SelectedIndex >= 0)
            {
                var selectedEmployee = employees[EmployeeComboBox.SelectedIndex];
                DatabaseHelper.DeleteEmployee(selectedEmployee.Id);
                MessageBox.Show($"{selectedEmployee.Name} a sido eliminado del sistema");
                LoadEmployees();
                RoleTextBlock.Text = string.Empty;
            }
            else
            {
                MessageBox.Show("Por Favor selecciona un empleado para borrar.");
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
