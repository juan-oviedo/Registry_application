using Registro.Classes;
using Registro.Properties;
using Registro.Windows;
using System.Windows;
using Application = System.Windows.Application;

namespace Registro
{
    /// <summary>
    /// Represents the application entry point and contains application-level events and logic.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Handles the <see cref="Application.Startup"/> event, which is triggered when the application starts.
        /// </summary>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DatabaseHelper.CreateDatabase();

            if (Settings.Default.FirstTime)
            {
                FirstRunWindow firstRunWindow = new FirstRunWindow();
                firstRunWindow.Show();
                Settings.Default.FirstTime = false;
                Settings.Default.Save();

            }
            else
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
        }
    }

}
