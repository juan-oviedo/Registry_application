using Registro.Constants;
using Registro.Properties;
using System.Data.SQLite;
using System.IO;

namespace Registro.Classes
{
    /// <summary>
    /// Provides helper methods for database operations, including creating and updating the database,
    /// and performing CRUD operations on employee data and check-ins.
    /// </summary>
    internal class DatabaseHelper
    {
        private const string connectionString = "Data Source=Empleados.db";
        private const string dataBaseName = "Empleados.db";
        private const string employees = "Empleados";
        private const string checkIns = "CheckIns";
        private const string lateMinutes = "LateMinutes";

        /// <summary>
        /// Creates the database file and the employees, check-ins, and late minutes tables if they do not exist.
        /// Also adds the "Deleted" column to the employees table if it is not already present.
        /// </summary>
        public static void CreateDatabase()
        {
            if (!File.Exists(dataBaseName))
            {
                // This will create the database file if it does not exist
                SQLiteConnection.CreateFile(dataBaseName);

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string createTableQuery = $@"CREATE TABLE IF NOT EXISTS {employees} ( " +
                                                "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                                "Name TEXT NOT NULL, " +
                                                "Position TEXT NOT NULL, " +
                                                "Password TEXT, " +
                                                $"Deleted INTEGER DEFAULT {DeletedConstants.NonDeleted}" +
                                                ");";

                    using (var command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    createTableQuery = $@"CREATE TABLE IF NOT EXISTS {checkIns} ( " +
                                                "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                                "EmployeeId INTEGER NOT NULL, " +
                                                "Type INTEGER NOT NULL, " +
                                                "CheckInTime TEXT NOT NULL, " +
                                                $"FOREIGN KEY (EmployeeId) REFERENCES {employees}(Id)" +
                                                ");";

                    using (var command1 = new SQLiteCommand(createTableQuery, connection))
                    {
                        command1.ExecuteNonQuery();
                    }

                    createTableQuery = $@" CREATE TABLE IF NOT EXISTS {lateMinutes} (" +
                                        "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                        "EmployeeId INTEGER NOT NULL, " +
                                        "Type INTEGER NOT NULL, " +
                                        "Shift TEXT NOT NULL, " +
                                        "Date TEXT NOT NULL, " +
                                        "LateMinutes INTEGER NOT NULL, " +
                                        "FOREIGN KEY(EmployeeId) REFERENCES Employees(Id) " +
                                        ");";

                    using (var command2 = new SQLiteCommand(createTableQuery, connection))
                    {
                        command2.ExecuteNonQuery();
                    }
                }

                Settings.Default.DataBaseActualized = true;
            }
            else if (!Settings.Default.DataBaseActualized)
            {
                AddDeletedColumnToEmployees();
                Settings.Default.DataBaseActualized = true;
            }
        }

        /// <summary>
        /// Adds the "Deleted" column to the employees table if it does not already exist.
        /// </summary>
        public static void AddDeletedColumnToEmployees()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Check if the 'deleted' column already exists
                string checkColumnQuery = $"PRAGMA table_info({employees});";
                bool columnExists = false;

                using (var command = new SQLiteCommand(checkColumnQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["name"].ToString() == "Deleted")
                            {
                                columnExists = true;
                                break;
                            }
                        }
                    }
                }

                // If the 'deleted' column does not exist, add it
                if (!columnExists)
                {
                    string addColumnQuery = $"ALTER TABLE {employees} ADD COLUMN Deleted INTEGER DEFAULT {DeletedConstants.NonDeleted};";

                    using (var command = new SQLiteCommand(addColumnQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Inserts a new employee record into the employees table.
        /// </summary>
        /// <param name="Name">The name of the employee to insert.</param>
        /// <param name="position">The position of the employee (e.g., Manager, Employee).</param>
        /// <param name="password">The password for the employee (optional, only required for certain roles).</param>
        /// <exception cref="InvalidOperationException">Thrown when the insert operation does not affect any rows.</exception>
        public static void InsertEmployee(string Name, string position, string? password = null)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {

                connection.Open();

                string insertQuery;
                if (password != null && (position == EmployeeRoles.Owner || position == EmployeeRoles.Manager))
                {
                    insertQuery = $@"INSERT INTO {employees} (Name, Position, Password) VALUES (@Name, @Position, @Password);";
                }
                else
                {
                    insertQuery = $@"INSERT INTO {employees} (Name, Position) VALUES (@Name, @Position);";
                }

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Position", position);
                    if (password != null) command.Parameters.AddWithValue("@Password", PasswordHelper.HashPassword(password));

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected <= 0)
                    {
                        throw new InvalidOperationException("The operation did not affect any rows.");
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves a list of non-deleted employees with their roles.
        /// </summary>
        /// <param name="isDev">Indicates whether to include the "Owner" role in the result.</param>
        /// <returns>A list of tuples containing the employee ID, name, and position.</returns>
        public static List<(int, string, string)> GetNonDeletedEmployeesWithRoles(bool isDev)
        {
            var emp = new List<(int, string, string)>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string selectQuery;
                if (isDev) selectQuery = $@"SELECT Id, Name, Position FROM {employees} WHERE Deleted = {DeletedConstants.NonDeleted} AND (Position = '{EmployeeRoles.Manager}' OR Position = '{EmployeeRoles.Employee}' OR Position = '{EmployeeRoles.Owner}');";
                else selectQuery = $@"SELECT Id, Name, Position FROM {employees} WHERE Deleted = {DeletedConstants.NonDeleted} AND (Position = '{EmployeeRoles.Manager}' OR Position = '{EmployeeRoles.Employee}');";

                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            string position = reader.GetString(2);
                            emp.Add((id, name, position));
                        }
                    }
                }
            }
            return emp;
        }

        /// <summary>
        /// Retrieves a list of all employees with passwords.
        /// </summary>
        /// <returns>A list of tuples containing the employee ID, name, position, and password.</returns>
        public static List<(int Id, string Name, string Position, string Password)> GetAllEmployeesPasswords()
        {
            var emp = new List<(int Id, string Name, string Position, string Password)>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string selectQuery = $"SELECT * FROM {employees}  WHERE Password IS NOT NULL AND Deleted = {DeletedConstants.NonDeleted};";

                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            string position = reader.GetString(2);
                            string password = reader.GetString(3);
                            emp.Add((id, name, position, password));
                        }
                    }
                }
            }
            return emp;
        }

        /// <summary>
        /// Marks an employee as deleted by setting the `deleted` flag in the database.
        /// </summary>
        /// <param name="employeeId">The ID of the employee to delete.</param>
        public static void DeleteEmployee(int employeeId)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string updateQuery = $"UPDATE {employees} SET deleted = {DeletedConstants.Deleted} WHERE Id = @employeeId";

                using (var command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@employeeId", employeeId);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts a check-in or check-out record for an employee and returns the time of the operation.
        /// </summary>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <param name="type">The type of check (Entry/Exit).</param>
        /// <returns>The time of the check-in or check-out in "HH:mm" format.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the operation did not affect any rows.</exception>
        public static string InsertCheck(int employeeId, int type)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string insertQuery = $"INSERT INTO {checkIns} (EmployeeId, Type, CheckInTime) VALUES (@EmployeeId, @Type, @CheckInTime);";
                DateTime time = DateTime.Now;
                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);
                    command.Parameters.AddWithValue("@Type", type);
                    command.Parameters.AddWithValue("@CheckInTime", time.ToString("yyyy-MM-dd HH:mm:ss"));

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected <= 0)
                    {
                        throw new InvalidOperationException("The operation did not affect any rows.");
                    }
                }
                return time.ToString("HH:mm");
            }
        }

        /// <summary>
        /// Retrieves a list of employee IDs who have checked in today within a specific time range.
        /// </summary>
        /// <returns>A list of employee IDs.</returns>
        public static List<int> GetAllEmployeesCheckedIn()
        {
            var checkIn = new List<int>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string selectQuery = $@"SELECT EmployeeId FROM {checkIns} WHERE CheckInTime BETWEEN @min AND @max ;";


                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    DateTime min, max;

                    if (TimeHelper.IsBeforeChangeTime())
                    {
                        min = DateTime.Today;
                        max = TimeHelper.GetTodayWithSpecificTime(
                            TimeSpan.Parse(Settings.Default.ChangeTime).Hours,
                            TimeSpan.Parse(Settings.Default.ChangeTime).Minutes);
                    }
                    else
                    {
                        min = TimeHelper.GetTodayWithSpecificTime(
                            TimeSpan.Parse(Settings.Default.ChangeTime).Hours,
                            TimeSpan.Parse(Settings.Default.ChangeTime).Minutes);
                        max = TimeHelper.GetTodayWithSpecificTime(23, 59);
                    }

                    command.Parameters.AddWithValue("@min", min.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@max", max.ToString("yyyy-MM-dd HH:mm:ss"));


                    using (var reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            checkIn.Add(id);
                        }
                    }
                }
            }
            return checkIn;
        }

        /// <summary>
        /// Retrieves a list of employees who have checked in but not checked out today.
        /// </summary>
        /// <returns>A list of tuples containing employee IDs and names.</returns>
        public static List<(int Id, string Name)> GetAllEmployeesCheckedInAndNotCheckedOut()
        {
            var checkedInNotOutEmployees = new List<(int, string)>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string selectQuery = $@"SELECT e.Id, e.Name " +
                                        $"FROM {employees} e " +
                                        $"JOIN {checkIns} ci_checkin " +
                                        "ON e.Id = ci_checkin.EmployeeId AND " +
                                        $"ci_checkin.Type = {TypeCheck.Entry} AND " +
                                        "ci_checkin.CheckInTime BETWEEN @min AND @max " +
                                        $"LEFT JOIN {checkIns} ci_checkout " +
                                        "ON e.Id = ci_checkout.EmployeeId AND " +
                                        $"ci_checkout.Type = {TypeCheck.Exit} AND " +
                                        "ci_checkout.CheckInTime BETWEEN @min AND @max " +
                                        "WHERE ci_checkout.Id IS NULL;";

                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    DateTime min, max;

                    if (TimeHelper.IsBeforeChangeTime())
                    {
                        min = DateTime.Today;
                        max = TimeHelper.GetTodayWithSpecificTime(
                            TimeSpan.Parse(Settings.Default.ChangeTime).Hours,
                            TimeSpan.Parse(Settings.Default.ChangeTime).Minutes);
                    }
                    else
                    {
                        min = TimeHelper.GetTodayWithSpecificTime(
                            TimeSpan.Parse(Settings.Default.ChangeTime).Hours,
                            TimeSpan.Parse(Settings.Default.ChangeTime).Minutes);
                        max = TimeHelper.GetTodayWithSpecificTime(23, 59);
                    }

                    command.Parameters.AddWithValue("@min", min.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@max", max.ToString("yyyy-MM-dd HH:mm:ss"));

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            checkedInNotOutEmployees.Add((id, name));
                        }
                    }
                }
            }
            return checkedInNotOutEmployees;
        }

        /// <summary>
        /// Retrieves a list of employees who have not checked in today within a specific time range.
        /// </summary>
        /// <returns>A list of tuples containing employee IDs and names.</returns>
        public static List<(int Id, string Name)> GetAllEmployeesNotCheckedIn()
        {
            var notCheckedIn = new List<(int Id, string Name)>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string selectQuery = $@"SELECT e.Id, e.Name FROM {employees} e " +
                                        $"LEFT JOIN {checkIns} c " +
                                        "ON e.Id = c.EmployeeId " +
                                        "AND c.CheckInTime BETWEEN @min AND @max " +
                                        $"WHERE e.Position <> '{EmployeeRoles.Owner}' " +
                                        "AND c.EmployeeId IS NULL " +
                                        $"AND e.Deleted = {DeletedConstants.NonDeleted};";

                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    DateTime min, max;

                    if (TimeHelper.IsBeforeChangeTime())
                    {
                        min = DateTime.Today;
                        max = TimeHelper.GetTodayWithSpecificTime(
                            TimeSpan.Parse(Settings.Default.ChangeTime).Hours,
                            TimeSpan.Parse(Settings.Default.ChangeTime).Minutes);
                    }
                    else
                    {
                        min = TimeHelper.GetTodayWithSpecificTime(
                            TimeSpan.Parse(Settings.Default.ChangeTime).Hours,
                            TimeSpan.Parse(Settings.Default.ChangeTime).Minutes);
                        max = TimeHelper.GetTodayWithSpecificTime(23, 59);
                    }

                    command.Parameters.AddWithValue("@min", min.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@max", max.ToString("yyyy-MM-dd HH:mm:ss"));

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            notCheckedIn.Add((id, name));
                        }
                    }
                }
            }

            return notCheckedIn;
        }

        /// <summary>
        /// Retrieves the opening time based on the first check-in by an owner or manager today.
        /// </summary>
        /// <returns>The DateTime of the first check-in.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no check-ins are found for the current shift.</exception>
        public static DateTime GetOpening()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = $@"SELECT c.CheckInTime " +
                                 $"FROM {checkIns} c " +
                                 $"INNER JOIN {employees} e " +
                                 "ON c.EmployeeId = e.Id " +
                                 $"WHERE e.Position IN ('{EmployeeRoles.Owner}', '{EmployeeRoles.Manager}') " +
                                 "AND c.CheckInTime BETWEEN @StartTime AND @EndTime " +
                                 "ORDER BY c.CheckInTime ASC " +
                                 "LIMIT 1;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    DateTime start, end;

                    if (TimeHelper.IsBeforeChangeTime())
                    {
                        start = DateTime.Today;
                        end = TimeHelper.GetTodayWithSpecificTime(
                            TimeSpan.Parse(Settings.Default.ChangeTime).Hours,
                            TimeSpan.Parse(Settings.Default.ChangeTime).Minutes);
                    }
                    else
                    {
                        start = TimeHelper.GetTodayWithSpecificTime(
                            TimeSpan.Parse(Settings.Default.ChangeTime).Hours,
                            TimeSpan.Parse(Settings.Default.ChangeTime).Minutes);
                        end = TimeHelper.GetTodayWithSpecificTime(23, 59);
                    }

                    command.Parameters.AddWithValue("@StartTime", start.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@EndTime", end.ToString("yyyy-MM-dd HH:mm:ss"));

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string time = reader.GetString(0);
                            DateTime checkInTime = DateTime.Parse(time);

                            return checkInTime;
                        }
                        else
                        {
                            throw new InvalidOperationException("No check-ins found for today shift.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Inserts a record of late minutes for an employee on a given date and shift.
        /// </summary>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <param name="type">The type of check (Entry/Exit).</param>
        /// <param name="date">The date of the late occurrence.</param>
        /// <param name="minutsLate">The number of minutes the employee was late.</param>
        /// <exception cref="InvalidOperationException">Thrown if the operation did not affect any rows.</exception>
        public static void InsertLateMinutes(int employeeId, int type, DateTime date, int minutsLate)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string insertQuery = $@"INSERT INTO {lateMinutes} (EmployeeId, Type,  Shift, Date, LateMinutes) VALUES (@EmployeeId, @Type, @Shift, @Date, @LateMinutes);";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);
                    command.Parameters.AddWithValue("@Type", type);
                    command.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@LateMinutes", minutsLate);
                    if (TimeHelper.IsBeforeChangeTime())
                    {
                        command.Parameters.AddWithValue("@Shift", Shift.Morning);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@Shift", Shift.Afternoon);
                    }

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected <= 0)
                    {
                        throw new InvalidOperationException("The operation did not affect any rows.");
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves a list of all employees' late minutes records within a specified date range.
        /// </summary>
        /// <param name="startDate">The start date of the range.</param>
        /// <param name="endDate">The end date of the range.</param>
        /// <returns>A list of tuples containing employee name, check type, shift, date, and late minutes.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no entries are found for the provided date range.</exception>
        public static List<(string Name, int type, string shift, DateTime date, int late)> GetAllEmployeeLateMinutes(DateTime startDate, DateTime endDate)
        {
            var list = new List<(string Name, int type, string shift, DateTime date, int late)>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string selectQuery = $@"SELECT e.Name, lm.Type, lm.Shift, lm.Date, lm.LateMinutes " +
                                        $"FROM {lateMinutes} lm " +
                                        $"JOIN {employees} e ON lm.EmployeeId = e.Id " +
                                        "WHERE lm.Date BETWEEN @StartDate AND @EndDate;";

                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd"));

                    using (var reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            string name = reader.GetString(0);
                            int type = reader.GetInt32(1);
                            string shift = reader.GetString(2);
                            DateTime date = DateTime.Parse(reader.GetString(3));
                            int late = reader.GetInt32(4);

                            list.Add((name, type, shift, date, late));
                        }
                        if (list.Count <= 0)
                        {
                            throw new InvalidOperationException("No entrys found for the date provided.");
                        }
                    }
                }
            }

            return list;
        }
    }
}
