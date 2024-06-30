using System.Globalization;
using System.Text.RegularExpressions;

namespace SlotBookingApp.Infrastructure.Helpers;

public class DateHelper
{
    public string GetWeeksMonday(string inputString)
    {
        string pattern = @"^(\d{4})-(\d{2})-(\d{2}).*";
        string formattedDate = Regex.Replace(inputString, pattern, "$1/$2/$3");

        DateTime inputDate = DateTime.Parse(formattedDate, CultureInfo.InvariantCulture);
        int daysToSubtract = ((int)inputDate.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        DateTime monday = inputDate.AddDays(-daysToSubtract);

        return monday.ToString("yyyyMMdd");
    }
}
