namespace BusinessLayer.Helpers
{
    using System.Text.RegularExpressions;

    public static class StringConverter
    {
        public static string ToPascalCase(this string value)
        {
            var regex = new Regex(@"\b[a-z]");

            return regex.Replace(value, match => match.Value.ToUpper());
        }
    }
}