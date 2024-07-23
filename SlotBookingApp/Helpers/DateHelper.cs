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
        if (!DateTime.TryParseExact(inputString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime inputDate))
        {
            throw new FormatException($"The date '{inputString}' should be in the following format: yyyy-MM-dd");
        }

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
    /// <exception cref="ArgumentException">Thrown when the intervalMinutes is less than or equal to 0.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the multiples of intervalMinutes do not add to end time.</exception>
    public List<string> GenerateTimeList(DateTime start, DateTime end, int intervalMinutes)
    {
        try
        {
            if (intervalMinutes <= 0)
            {
                throw new ArgumentException("Interval minutes must be a positive value.");
            }

            List<string> result = new List<string>();

            DateTime current = start;
            while (current < end && current > DateTime.Now)
            {
                DateTime nextToCurrent = current.AddMinutes(intervalMinutes);
                if (nextToCurrent > end)
                {
                    throw new ArgumentOutOfRangeException("The interval end time exceeds the specified end time.");
                }
                result.Add($"{current} - {nextToCurrent}");
                current = nextToCurrent;
            }
            return result;
        }
        catch (Exception ex)
        {
            throw new ArgumentException("An error occurred while generating the time list.", ex);
        }
    }
}