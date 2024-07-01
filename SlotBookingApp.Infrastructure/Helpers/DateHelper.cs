using System.Globalization;
using System.Text.RegularExpressions;

namespace SlotBookingApp.Infrastructure.Helpers;

public class DateHelper
{
    public DateTime GetWeeksMonday(string inputString)
    {
        string pattern = @"^(\d{4})-(\d{2})-(\d{2}).*";
        string formattedDate = Regex.Replace(inputString, pattern, "$1/$2/$3");

        DateTime inputDate = DateTime.Parse(formattedDate, CultureInfo.InvariantCulture);
        int daysToSubtract = ((int)inputDate.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        DateTime monday = inputDate.AddDays(-daysToSubtract);
        return monday;
    }

    public List<string> GenerateTimeList(DateTime start, DateTime end, int intervalMinutes)
    {
        List<string> result = new List<string>();

        DateTime current = start;
        while (current < end)
        {
            result.Add($"{current} - {current.AddMinutes(intervalMinutes)}");
            current = current.AddMinutes(intervalMinutes);
        }
        return result;
    }
}
