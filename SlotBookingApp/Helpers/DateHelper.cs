using SlotBookingApp.Infrastructure.Dtos;
using System.Globalization;
using System.Text.RegularExpressions;


namespace SlotBookingApp.Helpers;

/// <summary>
/// Helper class for date and time related operations.
/// </summary>
public class DateHelper
{
    /// <summary>
    /// Gets the Monday date of the week for a given input string date.
    /// </summary>
    /// <param name="inputString">The input date string in the format "yyyy-MM-dd".</param>
    /// <returns>The DateTime object representing the Monday date of the week.</returns>
    /// <exception cref="FormatException">Thrown when the inputString is not in the correct format.</exception>
    public DateTime GetWeeksMonday(string inputString)
    {
        if (!DateTime.TryParseExact(inputString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
        {
            throw new FormatException($"The date '{inputString}' should be in the following format: yyyy-MM-dd");
        }

        string pattern = @"^(\d{4})-(\d{2})-(\d{2}).*";
        string formattedDate = Regex.Replace(inputString, pattern, "$1/$2/$3");

        DateTime inputDate = DateTime.Parse(formattedDate, CultureInfo.InvariantCulture);
        int daysToSubtract = ((int)inputDate.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        DateTime monday = inputDate.AddDays(-daysToSubtract);
        return monday;
    }

    /// <summary>
    /// Generates a list of time intervals between start and end times with the specified interval in minutes.
    /// </summary>
    /// <param name="start">The start time of the interval.</param>
    /// <param name="end">The end time of the interval.</param>
    /// <param name="intervalMinutes">The interval in minutes between each time slot.</param>
    /// <returns>A list of strings representing time intervals.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the intervalMinutes is less than or equal to 0.</exception>
    public List<string> GenerateTimeList(DateTime start, DateTime end, int intervalMinutes)
    {
        try
        {
            List<string> result = new List<string>();

            DateTime current = start;
            while (current < end && current > DateTime.Now)
            {
                result.Add($"{current} - {current.AddMinutes(intervalMinutes)}");
                current = current.AddMinutes(intervalMinutes);
            }
            return result;
        }
        catch (Exception ex)
        {
            return new List<string>();
        }
    }
}
