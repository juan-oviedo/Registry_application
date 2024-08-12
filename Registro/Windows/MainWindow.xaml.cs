using Registro.Pages;
using System.Windows;

namespace Registro.Windows
{
    /// <summary>
    /// Represents the main application window that serves as the entry point of the user interface after the application starts.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new LogIn());
        }
    }
}