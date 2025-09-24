namespace Infrastructure.Extensions
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var attribute = enumValue.GetType()
                             .GetMember(enumValue.ToString())
                             .First()
                             .GetCustomAttribute<DisplayAttribute>();

            if (attribute != null)
            {
                return attribute.GetName();
            }

            return string.Empty;
        }
    }
}
