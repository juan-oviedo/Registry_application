using UserControl = System.Windows.Controls.UserControl;

namespace Registro.UserControls
{
    /// <summary>
    /// Interaction logic for TimePicker.xaml.
    /// This UserControl provides a simple way for users to select a time using separate combo boxes for hours and minutes.
    /// </summary>
    public partial class TimePicker : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimePicker"/> class.
        /// Sets up the UI components and populates the combo boxes for hours and minutes.
        /// </summary>
        public TimePicker()
        {
            InitializeComponent();
            InitializeTimePickers();
        }

        /// <summary>
        /// Populates the Hours and Minutes combo boxes with values.
        /// The Hours combo box is populated with values from 00 to 23,
        /// and the Minutes combo box is populated with values from 00 to 59.
        /// </summary>
        private void InitializeTimePickers()
        {
            for (int i = 0; i < 24; i++)
            {
                HoursComboBox.Items.Add(i.ToString("00"));
            }

            for (int i = 0; i < 60; i++)
            {
                MinutesComboBox.Items.Add(i.ToString("00"));
            }
        }

        /// <summary>
        /// Gets or sets the selected time in the format "HH:mm".
        /// When setting the time, the combo boxes are updated to reflect the provided time.
        /// </summary>
        public string SelectedTime
        {
            get { return $"{HoursComboBox.SelectedItem}:{MinutesComboBox.SelectedItem}"; }
            set
            {
                if (value != null)
                {
                    var timeParts = value.Split(':');
                    HoursComboBox.SelectedItem = timeParts[0];
                    MinutesComboBox.SelectedItem = timeParts[1];
                }
            }
        }
    }
}
