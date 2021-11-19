using System;

namespace CryptoTrader.Utils
{
    public static class DateTimeUtils
    {
        public static string ToRelativeString(this TimeSpan time)
        {
            string output = String.Empty;

            if (time.Days > 0)
            {
                // string days = time.Days != 1 ? "days" : "day";
                output += time.Days + $"d ";
            }

            if ((time.Days == 0 || time.Days == 1) && time.Hours > 0)
            {
                // string hours = time.Hours != 1 ? "hours" : "hour";
                output += time.Hours + $"h ";
            }

            if (time.Days == 0 && time.Minutes > 0)
            {
                // string mins = time.Minutes != 1 ? "mins" : "min";
                output += time.Minutes + $"m ";
            }

            if ((time.Days == 0 && time.Hours == 0 && time.Seconds > 0) || output.Length == 0)
            {
                // string secs = time.Seconds != 1 ? "secs" : "sec";
                output += time.Seconds + $"s ";
            }

            return output.Trim();
        }
        
        public static string ToRelativeString(this TimeSpan? time)
        {
            if (!time.HasValue)
                return "None";
            
            return ToRelativeString(time.Value);
        }
    }
}
