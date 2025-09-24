namespace Infrastructure.SQLDB.Extensions
{
    public static class LocationExtensions
    {
        public static string TransformCountry(this string country)
        {
            if (string.IsNullOrEmpty(country))
                return country;

            if (country.Equals("TAIWAN, PROVINCE OF CHINA", StringComparison.OrdinalIgnoreCase))
                return "Taiwan";
            else if (country.Equals("HONG KONG SPECIAL ADMINISTRATIVE REGION", StringComparison.OrdinalIgnoreCase))
                return "Hong Kong";

            return country;
        }
    }
}
