namespace Infrastructure.Configuration.Model
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.RegularExpressions;

    using Microsoft.Extensions.Configuration;

    [ExcludeFromCodeCoverage]
    public class MongoDbOptions : IConfigurationOptions
    {
        public string ConfigKey { get; } = "MongoDb";

        public string ConnectionString { get; set; } = null!;

        public string? Username { get; set; }

        public TimeSpan HealthcheckTimeout { get; set; }

        public int RetryBaseDelayMilliseconds { get; set; }

        public int MaxRetryAttempts { get; set; }

        public string BuildConnectionString(IConfiguration configuration)
        {
            var authRegex = new Regex("(?<=//).*(?=@)");

            // Using a property for the password will just raise a medium severity issue with Checkmarx
            var connectionString = authRegex.Replace(ConnectionString, $"{Username}:{configuration.GetValue<string>($"{ConfigKey}:Password")}");

            return connectionString;
        }
    }
}
