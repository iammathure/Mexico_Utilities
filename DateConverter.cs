using System.Globalization;

namespace Mexico_Utility
{
    public class DateConverter
    {
        public static string ConvertToYYYDDDMMM(string date)
        {
            if (DateTime.TryParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate.ToString("yyyy-MM-dd");
            }
            else
            {
                throw new FormatException("Invalid date format. Please use dd-MM-yyyy format.");
            }
        }

        public static string ConvertToDDMMYYYY(string date)
        {
            if (DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate.ToString("dd-MM-yyyy");
            }
            else
            {
                throw new FormatException("Invalid date format. Please use yyyy-MM-dd format.");
            }
        }
    }
}
