using System;
using Npa.Accounting.Common.DateTimes;

namespace Npa.Accounting.Common.Cards
{
    public record Expiration
    {
        private const int VARIATION = 20;
        
        public int Month { get; }
        public int Year { get; }

        private Expiration()
        {
        }

        public Expiration(int month, int year)
        {
            ThrowIfNegative(month);
            ThrowIfNegative(year);
            if (year < 100)
            {
                year += (DateTime.Now.Year / 100) * 100;
            }
            if (!IsMonthValid(month))
            {
                throw new FormatException($"Months must be between 1 and 12");
            }

            if (!IsYearValid(year))
            {
                throw new FormatException($"Invalid expiration year");
            }
            Month = month;
            Year = year;
        }
        
        public static explicit operator Expiration(DateTime value) => new Expiration(value.Month, value.Year);
        
        public static implicit operator DateTime(Expiration exp) => exp.ToDate();
        
        /// <summary>
        /// Casts either MM/yy or MMyy to an expiration
        /// </summary>
        /// <param name="exp">string expressed as MM/yy or MMyy</param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public static explicit operator Expiration(string exp)
        {
            var date = DateTimeHelper.FromMMYY(exp);

            try
            {
                return new Expiration(date.Month, date.Year);
            }
            catch (FormatException ex)
            {
                throw new InvalidCastException("Invalid format. MM/yy or MMyy format required");
            }
            
        }

        public override string ToString() => ToDate().ToString("MM/yy");

        public DateTime ToDate() => new DateTime(Year, Month, 1).AddMonths(1).AddMilliseconds(-1);
        private static bool IsMonthValid(int month) => month >= 1 && month <= 12;

        private static bool IsYearValid(int year) =>
            (year > DateTime.Now.Year - VARIATION && year < DateTime.Now.Year + VARIATION);

        private static void ThrowIfNegative(int val)
        {
            if (val < 0)
            {
                throw new FormatException("Cannot be negative");
            }
        }
    }
}