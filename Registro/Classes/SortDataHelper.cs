using Registro.Constants;

namespace Registro.Classes
{
    /// <summary>
    /// A helper class for sorting and organizing employee check-in and check-out data.
    /// </summary>
    internal class SortDataHelper
    {
        /// <summary>
        /// Sorts and organizes a list of employee check-ins and check-outs by date and shift (morning or afternoon).
        /// </summary>
        /// <param name="list">A list of tuples containing employee name, check type, shift, date, and lateness.</param>
        /// <returns>A list of tuples where each tuple represents a day, containing separate lists for morning and afternoon shifts with employee check-in and check-out details.</returns>
        public static List<(List<(string Name, DateTime inDate, int inLate, DateTime outDate, int outLate)> morning,
                            List<(string Name, DateTime inDate, int inLate, DateTime outDate, int outLate)> after)>
            SortData(List<(string Name, int type, string shift, DateTime date, int late)> list)
        {
            var days = new List<(List<(string Name, DateTime inDate, int inLate, DateTime outDate, int outLate)> morning,
                              List<(string Name, DateTime inDate, int inLate, DateTime outDate, int outLate)> after)>();

            DateTime date = list[0].date;
            while (list.Count != 0)
            {
                var morning = new List<(string Name, DateTime inDate, int inLate, DateTime outDate, int outLate)>();
                var after = new List<(string Name, DateTime inDate, int inLate, DateTime outDate, int outLate)>();

                while (list.Count > 0 && list[0].date.Date == date.Date)
                {
                    var entry = list[0];
                    list.RemoveAt(0);

                    if (entry.shift == Shift.Morning)
                    {
                        if (entry.type == TypeCheck.Entry)
                        {
                            morning.Add((entry.Name, entry.date, entry.late, new DateTime(), 0));
                        }
                        else
                        {
                            var index = morning.FindIndex(x => x.Name == entry.Name && x.inDate.Date == entry.date.Date && x.outDate == new DateTime() && x.outLate == 0);
                            morning[index] = (morning[index].Name, morning[index].inDate, morning[index].inLate, entry.date, entry.late);
                        }
                    }
                    else
                    {
                        if (entry.type == TypeCheck.Entry)
                        {
                            after.Add((entry.Name, entry.date, entry.late, new DateTime(), 0));
                        }
                        else
                        {
                            var index = after.FindIndex(x => x.Name == entry.Name && x.inDate.Date == entry.date.Date && x.outDate == new DateTime() && x.outLate == 0);
                            after[index] = (after[index].Name, after[index].inDate, after[index].inLate, entry.date, entry.late);
                        }
                    }
                }

                days.Add((morning, after));

                if (list.Count > 0)
                {
                    date = list[0].date;
                }
            }

            return days;
        }
    }
}
