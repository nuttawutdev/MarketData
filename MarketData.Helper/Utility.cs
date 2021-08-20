using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Helper
{
    public class Utility
    {
        public enum MonthEnum
        {
            Undefined, // Required here even though it's not a valid month
            January,
            February,
            March,
            April,
            May,
            June,
            July,
            August,
            September,
            October,
            November,
            December
        }

        public static DateTime GetDateNowThai()
        {
            var dateUtc = DateTime.UtcNow;
            return dateUtc.AddHours(7);
        }
    }
}
