using System;
using System.Text.RegularExpressions;

namespace Npa.Accounting.Common.DateTimes
{
    public static class DateTimeHelper
    {
        private static readonly Regex regex = new Regex(@"^(0[1-9]{1}|1[0-2]{1})/\d{2}$", RegexOptions.Compiled | RegexOptions.Singleline);
        public static DateTime GetLastDayOfTheMonth(DayOfWeek dayOfWeek, Month month, int year)
        {
            var date = new DateTime(year, (int)month, DateTime.DaysInMonth(year, (int)month));
            while (date.DayOfWeek != dayOfWeek)
            {
                date = date.AddDays(-1.0);
            }
            return date;
        }

        public static DateTime? GetNthDayOfTheMonth(int dayNum, DayOfWeek dayOfWeek, Month month, int year)
        {
            if (dayNum < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(dayNum));
            }

            var date = new DateTime(year, (int)month, 1);
            var currentDayNum = 0;
            do
            {
                if (date.DayOfWeek == dayOfWeek)
                {
                    currentDayNum++;
                    if (currentDayNum == dayNum)
                    {
                        return date;
                    }
                }
                date = date.AddDays(1.0);
            } while (date.Month == (int)month);
            return null;
        }

        public static DateTime FromMMYY(string value)
        {
            if (!regex.IsMatch(value))
            {
                throw new ArgumentException($"{nameof(value)} must be in MM/yy format");
            }
            
            var split = value.Split("/");
            var month = int.Parse(split[0]);
            var year = int.Parse(split[1]) + (DateTime.Now.Year / 100) * 100;
            return new DateTime(year, month, 1);
        }
    }
}