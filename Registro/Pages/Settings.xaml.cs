using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Registro.Pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml.
    /// Provides the interface for configuring application settings, including file output, timings, and other preferences.
    /// </summary>
    public partial class Settings : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// Sets up the UI components and loads the current settings into the interface.
        /// </summary>
        public Settings()
        {
            InitializeComponent();
            LoadSettings();
        }

        /// <summary>
        /// Loads the saved settings from the application properties into the UI components.
        /// </summary>
        private void LoadSettings()
        {
            OutputDirectoryTextBlock.Text = Properties.Settings.Default.OutputDirectory;
            FileNameTextBox.Text = Properties.Settings.Default.OutputFileName;
            SpaceBetweenTurnsTextBox.Text = Properties.Settings.Default.SpaceBetweenTurns.ToString();
            MorningEntryTimePicker.SelectedTime = Properties.Settings.Default.MorningEntry;
            MorningExitTimePicker.SelectedTime = Properties.Settings.Default.MorningExit;
            AfternoonEntryTimePicker.SelectedTime = Properties.Settings.Default.AfternoonEntry;
            AfternoonExitTimePicker.SelectedTime = Properties.Settings.Default.AfternoonExit;
            ChangeTimeTimePicker.SelectedTime = Properties.Settings.Default.ChangeTime;
        }

        /// <summary>
        /// Handles the "Save" button click event.
        /// Saves the current settings from the UI components into the application properties.
        /// Displays a success message or an error message if saving fails.
        /// </summary>
        private void Btn_Save(object sender, RoutedEventArgs e)
        {
            try
            {
                Properties.Settings.Default.OutputDirectory = OutputDirectoryTextBlock.Text;
                Properties.Settings.Default.OutputFileName = FileNameTextBox.Text;
                Properties.Settings.Default.SpaceBetweenTurns = int.Parse(SpaceBetweenTurnsTextBox.Text);
                Properties.Settings.Default.MorningEntry = MorningEntryTimePicker.SelectedTime;
                Properties.Settings.Default.MorningExit = MorningExitTimePicker.SelectedTime;
                Properties.Settings.Default.AfternoonEntry = AfternoonEntryTimePicker.SelectedTime;
                Properties.Settings.Default.AfternoonExit = AfternoonExitTimePicker.SelectedTime;
                Properties.Settings.Default.ChangeTime = ChangeTimeTimePicker.SelectedTime;

                Properties.Settings.Default.Save();
                System.Windows.MessageBox.Show("Configuracion guardada con exito.");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ocurrio un error tratando de guardar las configuraciones: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles the "Select Directory" button click event.
        /// Opens a folder browser dialog to select the output directory for saving files.
        /// Updates the `OutputDirectoryTextBlock` with the selected path.
        /// </summary>
        private void Btn_SelectDirectory(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
            {
                OutputDirectoryTextBlock.Text = dialog.SelectedPath;
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