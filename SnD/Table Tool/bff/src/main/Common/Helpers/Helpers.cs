namespace Common.Helpers
{
    using System;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    public class Helpers
    {
        public static (int, int) SplitVersionString(string version)
        {
            if (!string.IsNullOrWhiteSpace(version) &&
                version.Count(x => x == '.') == 1 &&
                int.TryParse(version[..version.IndexOf('.')], out var majorVersion) &&
                int.TryParse(version[(version.IndexOf('.') + 1)..], out var minorVersion))
            {
                return (majorVersion, minorVersion);
            }

            return (-1, -1);
        }


        public static string FormatVersionString(int majorVersion, int minorVersion) => $"{majorVersion}.{minorVersion}";

        public static bool IsJsonValid(string item)
        {

            try
            {
                var jsonItem = JObject.Parse(item);

                if (!item.StartsWith('{') || !item.EndsWith('}'))
                {
                    return false;
                }
                if (!jsonItem.TryGetValue("region", out _) || !jsonItem.TryGetValue("product", out _) || !jsonItem.TryGetValue("tableType", out _))
                {
                    return false;
                }
                if (string.IsNullOrEmpty(jsonItem["region"].ToString().Trim()) || string.IsNullOrEmpty(jsonItem["product"].ToString().Trim()) || string.IsNullOrEmpty(jsonItem["tableType"].ToString().Trim()))
                {
                    return false;
                }

            }
            catch (Exception)
            {

                return false;
            }


            return true;
        }

    }
}
