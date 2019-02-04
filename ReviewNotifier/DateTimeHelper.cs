using System;

namespace ReviewNotifier
{
    public class DateTimeHelper
    {
        public static DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }

        public static string GetDateTimeInFormattedType(DateTime dateTime)
        {
            string longTime = dateTime.ToLongTimeString();
            string formattedTypeDate = $"'{dateTime.Year}/{dateTime.Month}/{dateTime.Day} {longTime}'";
            return formattedTypeDate;
        }
    }
}