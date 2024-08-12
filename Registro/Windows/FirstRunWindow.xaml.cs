using Registro.Pages;
using System.ComponentModel;
using System.Windows;
using Application = System.Windows.Application;

namespace Registro.Windows
{
    /// <summary>
    /// Represents a window that appears when the application is run for the first time or when specific conditions are met.
    /// This window typically handles initial setup or login before the main application window is displayed.
    /// </summary>
    public partial class FirstRunWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirstRunWindow"/> class.
        /// </summary>
        public FirstRunWindow()
        {
            // Check if there is an existing main window in the application.
            // If there is, and it is not this window, close the existing main window.
            if (Application.Current.MainWindow != null && Application.Current.MainWindow != this)
            {
                Application.Current.MainWindow.Close();
            }

            InitializeComponent();
            MainFrame.Navigate(new SettingsLogIn());
        }

        /// <summary>
        /// Handles the Closing event of the <see cref="FirstRunWindow"/>. 
        /// This event is triggered when the window is about to close.
        /// </summary>
        void FirstRunWindow_Closing(object sender, CancelEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
        }
    }
}
