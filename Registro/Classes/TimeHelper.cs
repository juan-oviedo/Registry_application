using Registro.Properties;

namespace Registro.Classes
{
    /// <summary>
    /// A helper class for handling time-related operations, such as determining whether the current time is before a specific change time, 
    /// or calculating minutes early or late for specific events like employee entry and exit.
    /// </summary>
    internal class TimeHelper
    {
        /// <summary>
        /// Checks if the current time is before the predefined change time.
        /// </summary>
        /// <returns>True if the current time is before the change time; otherwise, false.</returns>
        public static bool IsBeforeChangeTime()
        {
            // Parse the MorningExit time
            TimeSpan changeTime = TimeSpan.Parse(Settings.Default.ChangeTime);

            // Get the current time
            TimeSpan currentTime = DateTime.Now.TimeOfDay;

            // Compare current time with morning exit time
            return currentTime < changeTime;
        }

        /// <summary>
        /// Creates a DateTime object for today's date with a specific hour and minute.
        /// </summary>
        /// <param name="hour">The hour component of the time.</param>
        /// <param name="minute">The minute component of the time.</param>
        /// <returns>A DateTime object representing today's date with the specified time.</returns>
        public static DateTime GetTodayWithSpecificTime(int hour, int minute)
        {
            // Get the current date
            DateTime today = DateTime.Today;

            // Create a new DateTime object with the specific time
            DateTime specificTimeToday = new DateTime(today.Year, today.Month, today.Day, hour, minute, 0);

            return specificTimeToday;
        }

        /// <summary>
        /// Calculates the number of minutes early or late for the opening time, based on the current time.
        /// </summary>
        /// <returns>The number of minutes early (negative) or late (positive) for the opening time.</returns>
        public static int GetMinutsEarlyOrLateOpening()
        {
            int currentTime = (int)DateTime.Now.TimeOfDay.TotalMinutes;
            int timeTarget;
            if (IsBeforeChangeTime())
            {
                timeTarget = (int)TimeSpan.Parse(Settings.Default.MorningEntry).TotalMinutes;

            }
            else
            {
                timeTarget = (int)TimeSpan.Parse(Settings.Default.AfternoonEntry).TotalMinutes;
            }

            return currentTime - timeTarget;
        }

        /// <summary>
        /// Calculates the number of minutes early or late for an employee's entry time, based on the current time.
        /// </summary>
        /// <returns>The number of minutes early (negative) or late (positive) for the employee's entry time.</returns>
        public static int GetMinutsEarlyOrLateEmployeeEntry()
        {
            var time = DateTime.Now.TimeOfDay;
            int currentTime = (int)time.TotalMinutes;
            int timeTarget;
            if (IsBeforeChangeTime())
            {
                if (time < TimeSpan.Parse(Settings.Default.MorningEntry))
                {
                    timeTarget = (int)TimeSpan.Parse(Settings.Default.MorningEntry).TotalMinutes;
                }
                else
                {
                    timeTarget = (int)DatabaseHelper.GetOpening().TimeOfDay.TotalMinutes;
                }
            }
            else
            {
                if (time < TimeSpan.Parse(Settings.Default.AfternoonEntry))
                {
                    timeTarget = (int)TimeSpan.Parse(Settings.Default.AfternoonEntry).TotalMinutes;
                }
                else
                {
                    timeTarget = (int)DatabaseHelper.GetOpening().TimeOfDay.TotalMinutes;
                }
            }
            return currentTime - timeTarget;
        }

        /// <summary>
        /// Calculates the number of minutes early or late for an employee's exit time, based on the current time.
        /// </summary>
        /// <returns>The number of minutes early (negative) or late (positive) for the employee's exit time.</returns>
        public static int GetMinutsEarlyOrLateEmployeeExit()
        {
            var time = DateTime.Now.TimeOfDay;
            int currentTime = (int)time.TotalMinutes;
            int timeTarget;
            if (IsBeforeChangeTime())
            {
                timeTarget = (int)TimeSpan.Parse(Settings.Default.MorningExit).TotalMinutes;
            }
            else
            {
                timeTarget = (int)TimeSpan.Parse(Settings.Default.AfternoonExit).TotalMinutes;
            }
            return currentTime - timeTarget;
        }
    }
}
