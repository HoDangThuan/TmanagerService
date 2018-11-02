using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TmanagerService.Core.Extensions
{
    public static class DateTimeExtension
    {
        readonly static DateTime TimeBegin = Convert.ToDateTime("01-01-1900");
        public static string ToDate(this DateTime dateTime)
        {
            if (dateTime == null || dateTime < TimeBegin)
                return string.Empty;
            return dateTime.ToString("dd-MM-yyyy");
        }
        public static string ToDate(this DateTime? dateTime)
        {
            if (dateTime == null || dateTime < TimeBegin)
                return string.Empty;
            return ((DateTime) dateTime).ToString("dd-MM-yyyy");
        }
        public static string ToDateTimeFull(this DateTime dateTime)
        {
            if (dateTime == null || dateTime < TimeBegin)
                return string.Empty;
            return dateTime.ToString("dd-MM-yyyy HH:mm:ss");
        }
        public static string ToDateTimeFull(this DateTime? dateTime)
        {
            if (dateTime == null || dateTime < TimeBegin)
                return string.Empty;
            return ((DateTime) dateTime).ToString("dd-MM-yyyy HH:mm:ss");
        }
        public static DateTime ConvertDateTime(this string inputValue)
        {
            DateTime theDate = new DateTime();
            try
            {
                inputValue = Regex.Replace(inputValue, " \\(.*\\)$", "");
                theDate = DateTime.ParseExact(inputValue, "ddd MMM dd yyyy HH:mm:ss 'GMT'zzzz",
                    System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                try
                {
                    theDate = DateTime.Parse(inputValue);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return theDate;
        }
    }
}
