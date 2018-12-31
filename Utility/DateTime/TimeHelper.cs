using System;
using System.Globalization;

namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    /// ref: http://stackoverflow.com/questions/7908343/list-of-timezone-ids-for-use-with-findtimezonebyid-in-c
    /// 
    public static class TimeHelper
    {
        public static readonly TimeZoneInfo Local = TimeZoneInfo.Local;
        public static readonly TimeZoneInfo AEST = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");
        public static readonly TimeZoneInfo PST = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
        public static readonly TimeZoneInfo MST = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
        public static readonly TimeZoneInfo GMT = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        public static readonly TimeZoneInfo UTC = TimeZoneInfo.FindSystemTimeZoneById("UTC");

        public static DateTime ConvertToTimezone(DateTime dateTime, TimeZoneInfo sourceTimeZoneInfo, TimeZoneInfo destinationTimeZoneInfo)
        {
            return TimeZoneInfo.ConvertTime(dateTime, sourceTimeZoneInfo, destinationTimeZoneInfo);
        }

        /// <summary>
        /// Parse time from the string format "00:00"
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetOnlyTimeFromString(string time)
        {
            DateTime result = new DateTime();
            const string format = "HH:mm";
            CultureInfo provider = CultureInfo.InvariantCulture;
            try
            {
                result = DateTime.ParseExact(time, format, provider);
            }
            catch (FormatException e)
            {
                new Exception(string.Format("{0} is not in the correct format.", time), e);
            }

            return result;
        }

        /// <summary>
        /// Rounding ticks to a specific precision
        /// </summary>
        /// <param name="date"></param>
        /// <param name="roundTicks"></param>
        /// <returns></returns>
        public static DateTime Trim(this DateTime date, long roundTicks)
        {
            return new DateTime(date.Ticks - date.Ticks % roundTicks);
        }
    }
}