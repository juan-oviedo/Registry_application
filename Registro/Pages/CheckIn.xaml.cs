using Registro.Classes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MessageBox = System.Windows.MessageBox;


namespace Registro.Pages
{
    /// <summary>
    /// Interaction logic for CheckIn.xaml.
    /// Provides functionality for registering employee check-ins, ensuring that employees are only allowed to check in once per shift.
    /// </summary>
    public partial class CheckIn : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckIn"/> class.
        /// Sets up the UI components and loads the list of employees who haven't checked in yet.
        /// </summary>
        public CheckIn()
        {
            InitializeComponent();
            LoadEmployees();
        }

        /// <summary>
        /// Loads the list of employees who have not yet checked in and populates the `EmployeeComboBox`.
        /// </summary>
        private void LoadEmployees()
        {
            EmployeeComboBox.Items.Clear();

            var employees = DatabaseHelper.GetAllEmployeesNotCheckedIn();
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
        /// Handles the "Check In" button click event.
        /// Registers the selected employee's check-in for the current shift.
        /// If the employee has already checked in for this shift, a message is shown.
        /// Reloads the list of employees who haven't checked in after a successful check-in.
        /// </summary>
        private void Btn_CheckIn(object sender, RoutedEventArgs e)
        {
            if (EmployeeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                int employeeId = (int)selectedItem.Tag;
                try
                {
                    string? time = CheckHelper.CheckInForEmployee(employeeId);
                    LoadEmployees();
                    if (time != null) MessageBox.Show($"El check in de {selectedItem.Content} fue {time}.");
                    else MessageBox.Show($"{selectedItem.Content} ya registro su entrada por este turno.");
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
