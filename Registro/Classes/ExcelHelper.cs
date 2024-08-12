using OfficeOpenXml;
using OfficeOpenXml.Style;
using Registro.Properties;
using System.IO;

namespace Registro.Classes
{
    /// <summary>
    /// A helper class for generating Excel reports of employee check-ins, check-outs, and late minutes.
    /// </summary>
    internal class ExcelHelper
    {
        /// <summary>
        /// Generates an Excel file with two worksheets: one for recording employee check-ins and check-outs,
        /// and another for summarizing the late minutes for each employee.
        /// </summary>
        /// <param name="list">
        /// A list of tuples, where each tuple contains two lists:
        /// - The first list represents the morning shift and contains tuples of employee name, check-in time, minutes late, check-out time, and minutes late.
        /// - The second list represents the afternoon shift and contains similar tuples.
        /// </param>
        public static void GenerateExcelFile(List<(List<(string Name, DateTime inDate, int inLate, DateTime outDate, int outLate)> morning,
                                                    List<(string Name, DateTime inDate, int inLate, DateTime outDate, int outLate)> after)> list)
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("EntryAndExits");
            var worksheet2 = package.Workbook.Worksheets.Add("MinutsLate");

            AddSideHeaders(worksheet);
            AddHeadersMinuts(worksheet2);

            var employees = new List<(string Name, int earlyIn, int lateIn, int earlyOut, int lateOut)>();

            int i = 0;
            foreach (var day in list)
            {
                FillEntryAndExits(worksheet, i, day.morning, day.after);
                employees = AddEmployeesMinuts(day.morning, employees);
                employees = AddEmployeesMinuts(day.after, employees);
                i++;
            }

            FillMinutsLate(worksheet2, employees);

            // hay que centrar todo lo del documento
            CenterAllCells(worksheet);
            CenterAllCells(worksheet2);

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            worksheet2.Cells[worksheet2.Dimension.Address].AutoFitColumns();

            // Save to file
            var filePath = Path.Combine(Settings.Default.OutputDirectory, Settings.Default.OutputFileName + ".xlsx");
            var fileInfo = new FileInfo(filePath);
            package.SaveAs(fileInfo);

        }

        /// <summary>
        /// Fills the "EntryAndExits" worksheet with data for a specific day, including both morning and afternoon shifts.
        /// </summary>
        /// <param name="worksheet">The Excel worksheet to fill.</param>
        /// <param name="i">The day index to fill.</param>
        /// <param name="morning">A list of employee check-ins and check-outs for the morning shift.</param>
        /// <param name="after">A list of employee check-ins and check-outs for the afternoon shift.</param>
        private static void FillEntryAndExits(ExcelWorksheet worksheet, int i,
                                                    List<(string Name, DateTime inDate, int inLate, DateTime outDate, int outLate)> morning,
                                                    List<(string Name, DateTime inDate, int inLate, DateTime outDate, int outLate)> after)
        {
            int firstCell = i * 3;
            var range = worksheet.Cells[1, firstCell + 2, 1, firstCell + 4];
            range.Merge = true;
            range = worksheet.Cells[2, firstCell + 2, 2, firstCell + 4];
            range.Merge = true;

            if (morning.Count != 0)
            {
                worksheet.Cells[1, firstCell + 2].Value = morning[0].inDate.ToString("yyyy-MM-dd");
                worksheet.Cells[2, firstCell + 2].Value = morning[0].inDate.DayOfWeek.ToString();
                FillDataMorning(worksheet, firstCell, morning);

            }
            else if (after.Count != 0)
            {
                worksheet.Cells[1, firstCell + 2].Value = after[0].inDate.ToString("yyyy-MM-dd");
                worksheet.Cells[2, firstCell + 2].Value = after[0].inDate.DayOfWeek.ToString();
            }
            else
            {
                //
                // hay que agregar que levante una exepcion
                //
            }

            if (after.Count != 0)
            {
                FillDataAfter(worksheet, firstCell, after);
            }
        }

        /// <summary>
        /// Fills the "EntryAndExits" worksheet with data for the morning shift.
        /// </summary>
        /// <param name="worksheet">The Excel worksheet to fill.</param>
        /// <param name="firstCell">The first cell index for the morning shift.</param>
        /// <param name="morning">A list of employee check-ins and check-outs for the morning shift.</param>
        private static void FillDataMorning(ExcelWorksheet worksheet, int firstCell, List<(string Name, DateTime inDate, int inLate, DateTime outDate, int outLate)> morning)
        {
            worksheet.Cells[3, firstCell + 2].Value = "Persona:";
            worksheet.Cells[3, firstCell + 3].Value = "Entrada:";
            worksheet.Cells[3, firstCell + 4].Value = "Salida:";

            int j = 0;
            foreach (var entry in morning)
            {
                worksheet.Cells[j + 4, firstCell + 2].Value = entry.Name;
                worksheet.Cells[j + 4, firstCell + 3].Value = entry.inDate.TimeOfDay.ToString(@"hh\:mm");
                if (entry.outDate != new DateTime())
                {
                    worksheet.Cells[j + 4, firstCell + 4].Value = entry.outDate.TimeOfDay.ToString(@"hh\:mm");
                }
                j++;
            }
        }

        /// <summary>
        /// Fills the "EntryAndExits" worksheet with data for the afternoon shift.
        /// </summary>
        /// <param name="worksheet">The Excel worksheet to fill.</param>
        /// <param name="firstCell">The first cell index for the afternoon shift.</param>
        /// <param name="after">A list of employee check-ins and check-outs for the afternoon shift.</param>
        private static void FillDataAfter(ExcelWorksheet worksheet, int firstCell, List<(string Name, DateTime inDate, int inLate, DateTime outDate, int outLate)> after)
        {
            worksheet.Cells[6 + Settings.Default.SpaceBetweenTurns, firstCell + 2].Value = "Persona:";
            worksheet.Cells[6 + Settings.Default.SpaceBetweenTurns, firstCell + 3].Value = "Entrada:";
            worksheet.Cells[6 + Settings.Default.SpaceBetweenTurns, firstCell + 4].Value = "Salida:";

            int j = 0;
            foreach (var entry in after)
            {
                worksheet.Cells[7 + Settings.Default.SpaceBetweenTurns + j, firstCell + 2].Value = entry.Name;
                worksheet.Cells[7 + Settings.Default.SpaceBetweenTurns + j, firstCell + 3].Value = entry.inDate.TimeOfDay.ToString(@"hh\:mm");
                if (entry.outDate != new DateTime())
                {
                    worksheet.Cells[7 + Settings.Default.SpaceBetweenTurns + j, firstCell + 4].Value = entry.outDate.TimeOfDay.ToString(@"hh\:mm");
                }
                j++;
            }
        }

        /// <summary>
        /// Adds the late minutes for each employee to a list, which is used for the "MinutsLate" worksheet.
        /// </summary>
        /// <param name="list">A list of employee check-ins and check-outs.</param>
        /// <param name="employees">A list of employees with their accumulated late minutes.</param>
        /// <returns>The updated list of employees with accumulated late minutes.</returns>
        private static List<(string Name, int earlyIn, int lateIn, int earlyOut, int lateOut)> AddEmployeesMinuts(
                                                List<(string Name, DateTime inDate, int inLate, DateTime outDate, int outLate)> list,
                                                List<(string Name, int earlyIn, int lateIn, int earlyOut, int lateOut)> employees)
        {
            foreach (var employe in list)
            {
                int earlyIn = 0;
                int lateIn = 0;
                int earlyOut = 0;
                int lateOut = 0;


                if (employe.inLate >= 0) lateIn = employe.inLate;
                else earlyIn = -employe.inLate;
                if (employe.outLate >= 0) lateOut = employe.outLate;
                else earlyOut = -employe.outLate;

                var index = employees.FindIndex(x => x.Name == employe.Name);
                if (index >= 0) employees[index] = (employees[index].Name,
                                                    employees[index].earlyIn + earlyIn,
                                                    employees[index].lateIn + lateIn,
                                                    employees[index].earlyOut + earlyOut,
                                                    employees[index].lateOut + lateOut);
                else employees.Add((employe.Name, earlyIn, lateIn, earlyOut, lateOut));
            }
            return employees;
        }

        /// <summary>
        /// Fills the "MinutsLate" worksheet with the total early and late minutes for each employee.
        /// </summary>
        /// <param name="worksheet">The Excel worksheet to fill.</param>
        /// <param name="employees">A list of employees with their accumulated late minutes.</param>
        private static void FillMinutsLate(ExcelWorksheet worksheet, List<(string Name, int earlyIn, int lateIn, int earlyOut, int lateOut)> employees)
        {
            int i = 0;
            foreach (var employe in employees)
            {
                worksheet.Cells[i + 2, 1].Value = employe.Name;
                worksheet.Cells[i + 2, 2].Value = employe.earlyIn;
                worksheet.Cells[i + 2, 3].Value = employe.lateIn;
                worksheet.Cells[i + 2, 4].Value = employe.earlyOut;
                worksheet.Cells[i + 2, 5].Value = employe.lateOut;
                worksheet.Cells[i + 2, 6].Value = employe.lateIn + employe.earlyOut - employe.earlyIn - employe.lateOut;
                i++;
            }
        }

        /// <summary>
        /// Adds side headers to the "EntryAndExits" worksheet.
        /// </summary>
        /// <param name="worksheet">The Excel worksheet to add headers to.</param>
        private static void AddSideHeaders(ExcelWorksheet worksheet)
        {
            worksheet.Cells[3, 1].Value = "Mañana";
            worksheet.Cells[4, 1].Value = "Abrio:";
            worksheet.Cells[5, 1].Value = "Empleados:";

            worksheet.Cells[6 + Settings.Default.SpaceBetweenTurns, 1].Value = "Tarde:";
            worksheet.Cells[7 + Settings.Default.SpaceBetweenTurns, 1].Value = "Abrio:";
            worksheet.Cells[8 + Settings.Default.SpaceBetweenTurns, 1].Value = "Empleados:";
        }

        /// <summary>
        /// Adds headers to the "MinutsLate" worksheet.
        /// </summary>
        /// <param name="worksheet">The Excel worksheet to add headers to.</param>
        private static void AddHeadersMinuts(ExcelWorksheet worksheet)
        {
            worksheet.Cells[1, 2].Value = "Minutos temprano acumulados:";
            worksheet.Cells[1, 3].Value = "Minutos tardes acumulados:";
            worksheet.Cells[1, 4].Value = "Minutos que se fue temprano acumulados:";
            worksheet.Cells[1, 5].Value = "Minutos que se quedo extra acumulados:";
            worksheet.Cells[1, 6].Value = "Total:";
            worksheet.Cells[1, 7].Value = "Aclaracion";
            worksheet.Cells[2, 7].Value = "Si el total da como resultado positivo, son minutos que los empleados llegaron tarde";
            worksheet.Cells[3, 7].Value = "Si el total da como resultado negativo, son minutos que los empleados llegaron temprano";
        }

        /// <summary>
        /// Centers the content of all cells in the specified worksheet.
        /// </summary>
        /// <param name="worksheet">The Excel worksheet to center the content of.</param>
        private static void CenterAllCells(ExcelWorksheet worksheet)
        {
            ExcelRangeBase dimension = worksheet.Dimension;

            for (int row = dimension.Start.Row; row <= dimension.End.Row; row++)
            {
                for (int col = dimension.Start.Column; col <= dimension.End.Column; col++)
                {
                    ExcelRange cell = worksheet.Cells[row, col];
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
            }
        }
    }
}
