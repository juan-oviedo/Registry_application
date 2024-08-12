using Registro.Constants;

namespace Registro.Classes
{
    /// <summary>
    /// Provides helper methods for managing employee check-ins and check-outs.
    /// </summary>
    internal class CheckHelper
    {
        /// <summary>
        /// Registers a check-in for the start of the day. If the employee is already checked in, the method does nothing.
        /// </summary>
        /// <param name="id">The unique identifier of the employee.</param>
        public static void CheckInForOpening(int id)
        {
            var checkIn = DatabaseHelper.GetAllEmployeesCheckedIn();
            if (checkIn.Contains(id))
            {
                return;
            }
            DatabaseHelper.InsertCheck(id, TypeCheck.Entry);
            int minuts = TimeHelper.GetMinutsEarlyOrLateOpening();
            DatabaseHelper.InsertLateMinutes(id, TypeCheck.Entry, DateTime.Now, minuts);
        }

        /// <summary>
        /// Registers a check-in for an employee. If the employee is already checked in, the method returns null.
        /// </summary>
        /// <param name="id">The unique identifier of the employee.</param>
        /// <returns>The time the check-in was registered, or null if the employee was already checked in.</returns>
        public static string? CheckInForEmployee(int id)
        {
            var checkIn = DatabaseHelper.GetAllEmployeesCheckedIn();
            if (checkIn.Contains(id))
            {
                return null;
            }
            string Time = DatabaseHelper.InsertCheck(id, TypeCheck.Entry);
            int minuts = TimeHelper.GetMinutsEarlyOrLateEmployeeEntry();
            DatabaseHelper.InsertLateMinutes(id, TypeCheck.Entry, DateTime.Now, minuts);
            return Time;
        }

        /// <summary>
        /// Registers a check-out for an employee.
        /// </summary>
        /// <param name="id">The unique identifier of the employee.</param>
        /// <returns>The time the check-out was registered.</returns>
        public static string CheckOut(int id)
        {
            string Time = DatabaseHelper.InsertCheck(id, TypeCheck.Exit);
            int minuts = TimeHelper.GetMinutsEarlyOrLateEmployeeExit();
            DatabaseHelper.InsertLateMinutes(id, TypeCheck.Exit, DateTime.Now, minuts);
            return Time;
        }
    }
}
