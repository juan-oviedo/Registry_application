using Registro.Classes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MessageBox = System.Windows.MessageBox;

namespace Registro.Pages
{
    /// <summary>
    /// Interaction logic for CheckOut.xaml.
    /// Provides functionality for registering employee check-outs, ensuring that only employees who have checked in can check out.
    /// </summary>
    public partial class CheckOut : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckOut"/> class.
        /// Sets up the UI components and loads the list of employees who have checked in but not yet checked out.
        /// </summary>
        public CheckOut()
        {
            InitializeComponent();
            LoadEmployees();
        }

        /// <summary>
        /// Loads the list of employees who have checked in and not yet checked out, and populates the `EmployeeComboBox`.
        /// </summary>
        private void LoadEmployees()
        {
            EmployeeComboBox.Items.Clear();

            var employees = DatabaseHelper.GetAllEmployeesCheckedInAndNotCheckedOut();
            foreach (var employee in employees)
            {
                EmployeeComboBox.Items.Add(new ComboBoxItem
                {
                    Content = employee.Name,
                    Tag = employee.Id
                });
            }
        }

        /// <summary>
        /// Handles the "Check Out" button click event.
        /// Registers the selected employee's check-out and displays the check-out time.
        /// Reloads the list of employees who haven't checked out after a successful check-out.
        /// </summary>
        private void Btn_CheckOut(object sender, RoutedEventArgs e)
        {
            if (EmployeeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                int employeeId = (int)selectedItem.Tag;
                try
                {
                    string time = CheckHelper.CheckOut(employeeId);
                    LoadEmployees();
                    MessageBox.Show($"El check out de {selectedItem.Content} fue {time}.");
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show($"Por favor intentalo mas tarde.");
                }
            }
            else
            {
                MessageBox.Show("Por favor selecciona un empleado.");
            }
        }

        /// <summary>
        /// Handles the "Check Out All" button click event.
        /// Registers the check-out for all employees who have checked in and not yet checked out.
        /// Displays the check-out times for all these employees.
        /// Reloads the list of employees who haven't checked out after completing the process.
        /// </summary>
        private void Btn_CheckOutAll(object sender, RoutedEventArgs e)
        {
            var employees = DatabaseHelper.GetAllEmployeesCheckedInAndNotCheckedOut();
            var strig = "Se a hecho Check Out de:\n";
            try
            {
                foreach (var employee in employees)
                {
                    string time = CheckHelper.CheckOut(employee.Id);
                    strig += $"{employee.Name} tiempo: {time}\n";
                }
                LoadEmployees();
                MessageBox.Show(strig);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show($"Por favor intentalo mas tarde.");
            }
        }

        /// <summary>
        /// Handles the "Go Back" button click event.
        /// Navigates back to the previous page in the navigation history.
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
