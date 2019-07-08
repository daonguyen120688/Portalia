using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Portalia.Extentions
{
    public static class DateTimeExtention
    {
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static List<DateTime> ListDateInWeek(this DateTime dateTime)
        {
            int currentDayOfWeek = (int)dateTime.DayOfWeek;
            DateTime sunday = dateTime.AddDays(-currentDayOfWeek);
            DateTime monday = sunday.AddDays(1);
            if (currentDayOfWeek == 0)
            {
                monday = monday.AddDays(-7);
            }
            var dates = Enumerable.Range(0, 5).Select(days => monday.AddDays(days)).ToList();
            return dates.ToList();
        }
        public static IEnumerable<DateTime> DateRange(DateTime fromDate, DateTime toDate)
        {
            return Enumerable.Range(0, toDate.Subtract(fromDate).Days + 1)
                             .Select(d => fromDate.AddDays(d)).Where(time => time.DayOfWeek != DayOfWeek.Sunday && time.DayOfWeek != DayOfWeek.Saturday);
        }

        public static string ToStringWithSpecificFormat(this DateTime date, string format = "dd/MM/yyyy HH:mm:ss")
        {
            return date.ToString(format);
        }
    }
}