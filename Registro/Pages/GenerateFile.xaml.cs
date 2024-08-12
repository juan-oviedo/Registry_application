using Registro.Classes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MessageBox = System.Windows.MessageBox;

namespace Registro.Pages
{
    /// <summary>
    /// Interaction logic for GenerateFile.xaml.
    /// Provides functionality for generating an Excel file containing employee attendance data 
    /// within a specified date range.
    /// </summary>
    public partial class GenerateFile : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateFile"/> class.
        /// Sets up the UI components required for the page.
        /// </summary>
        public GenerateFile()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the "Generate" button click event.
        /// Generates an Excel file with employee attendance data within the selected date range.
        /// Validates that both start and end dates are selected before proceeding.
        /// </summary>
        private void Btn_Generate(object sender, RoutedEventArgs e)
        {
            DateTime? startDateNullable = StartDatePicker.SelectedDate;
            DateTime? endDateNullable = EndDatePicker.SelectedDate;

            if (startDateNullable == null || endDateNullable == null)
            {
                MessageBox.Show("Please select both start and end dates.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime startDate = startDateNullable.Value;
            DateTime endDate = endDateNullable.Value;
            List<(string Name, int type, string shift, DateTime date, int late)> list = new List<(string Name, int type, string shift, DateTime date, int late)>();

            try
            {
                list = DatabaseHelper.GetAllEmployeeLateMinutes(startDate, endDate);

            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("No se encontraron datos para la fecha seleccionada.");
                return;
            }

            var list2 = SortDataHelper.SortData(list);
            try
            {
                ExcelHelper.GenerateExcelFile(list2);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Por favor cierra el archivo Excel y re-intenta generar el archivo.");
                return;
            }
            MessageBox.Show($"El archivo fue generado desde {startDate.ToShortDateString()} hasta {endDate.ToShortDateString()}.");
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
