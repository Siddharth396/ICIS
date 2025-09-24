namespace Infrastructure.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;

    using JSNLog;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    ///     This is an adapter class to convert JSNMessages into structured message for Serilog.
    /// </summary>
    /// <remarks>
    ///     There is not a straight forward way to generate an instance of this class for Unit testing.
    ///     The FinalLogData class cannot be constructed since its Param types are marked as internal.
    ///     Hence excluding it from code coverage till the time we find a suitable solution.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    public class JSNLogToSerilogAdapter : ILoggingAdapter
    {
        private readonly ILoggerFactory loggerFactory;

        public JSNLogToSerilogAdapter(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        public void Log(FinalLogData finalLogData)
        {
            var logger = loggerFactory.CreateLogger(finalLogData.FinalLogger);
            var level = ConvertLevel(finalLogData.FinalLevel);

            if (!logger.IsEnabled(level))
            {
                return;
            }

            using (logger.BeginScope(new Dictionary<string, object> { { "Component", "Web" }, { "Service", "Web" } }))
            {
                var message = finalLogData.FinalMessage;

                var isJsonMessage = IsProbableJsonMessage(message);
                Dictionary<string, object>? logItems = null;

                if (isJsonMessage)
                {
                    try
                    {
                        logItems = FlattenJson(message);
                    }
                    catch
                    {
                        // Suppress any conversion errors and treat the message as regular string message
                        isJsonMessage = false;
                    }
                }

                var templateString = GetTemplateString(logItems);

                var logParams = isJsonMessage && logItems != null
                                    ? logItems.Values.ToArray()
                                    : new object[] { message };

                logger.Log(
                    level,
                    templateString,
                    logParams);
            }
        }

        private static bool AddToParams(Dictionary<string, object> paramsList, Dictionary<string, object>? infoToAdd)
        {
            var valueAdded = false;
            if (infoToAdd != null && infoToAdd.Any())
            {
                foreach (var info in infoToAdd)
                {
                    paramsList[info.Key] = info.Value;
                }

                valueAdded = true;
            }

            return valueAdded;
        }

        private static LogLevel ConvertLevel(Level level)
        {
            switch (level)
            {
                case Level.OFF:
                    return LogLevel.None;
                case Level.ALL:
                case Level.TRACE:
                    return LogLevel.Trace;
                case Level.DEBUG:
                    return LogLevel.Debug;
                case Level.INFO:
                    return LogLevel.Information;
                case Level.WARN:
                    return LogLevel.Warning;
                case Level.ERROR:
                    return LogLevel.Error;
                case Level.FATAL:
                default:
                    return LogLevel.Critical;
            }
        }

        private static Dictionary<string, object>? FlattenJson(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return null;
            }

            var paramsList = new Dictionary<string, object>();
            var deserializedMessage = JsonConvert.DeserializeObject<Dictionary<string, object>>(
                message,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

            foreach (var messageItem in deserializedMessage!)
            {
                if (TryParseDeviceInfo(paramsList, messageItem))
                {
                    continue;
                }

                if (TryParseTelemetryInfo(paramsList, messageItem))
                {
                    continue;
                }

                ParseGenericInfo(paramsList, messageItem);
            }

            return paramsList;
        }

        private static string GetLogFriendlyName(string original)
        {
            if (original.Equals("name", StringComparison.OrdinalIgnoreCase))
            {
                return "BrowserName";
            }

            if (original.Equals("version", StringComparison.OrdinalIgnoreCase))
            {
                return "BrowserVersion";
            }

            return original.Equals("fullVersion", StringComparison.OrdinalIgnoreCase)
                       ? "BrowserFullVersion"
                       : ToTitleCase(original);
        }

        private static string GetTemplateString(Dictionary<string, object>? logItems)
        {
            var keys = logItems?.Keys;

            if (keys == null || !keys.Any())
            {
                return "{Message}";
            }

            // do not duplicate the message
            if (keys.Contains("Message"))
            {
                var message = logItems?["Message"].ToString();
                logItems?.Remove("Message");
                return message!;
            }

            var templateStringBuilder = new StringBuilder();
            foreach (var key in keys)
            {
                templateStringBuilder.Append($"{{{key}}}");
            }

            return templateStringBuilder.ToString().Trim();
        }

        private static object GetValue(string stringValue)
        {
            var isInt = int.TryParse(stringValue, out var intValue);

            if (isInt)
            {
                return intValue;
            }

            return stringValue;
        }

        private static bool IsProbableJsonMessage(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                return false;
            }

            msg = msg.Trim();
            return msg.StartsWith("{") && msg.EndsWith("}");
        }

        private static void ParseGenericInfo(Dictionary<string, object> paramsList, KeyValuePair<string, object> item)
        {
            var value = item.Value switch
            {
                JObject jobj => JsonConvert.SerializeObject(jobj),
                JArray array => JsonConvert.SerializeObject(array),
                _ => GetValue(Convert.ToString(item.Value) ?? string.Empty)
            };

            paramsList[ToTitleCase(item.Key)] = value;
        }

        private static string ToTitleCase(string key)
        {
            var characters = key.ToCharArray();
            characters[0] = char.ToUpper(characters[0]);
            return new string(characters);
        }

        private static bool TryParseDeviceInfo(Dictionary<string, object> paramsList, KeyValuePair<string, object> item)
        {
            if (item.Key.Equals("device", StringComparison.InvariantCultureIgnoreCase))
            {
                var content = item.Value as JObject;
                var deviceInfo = content?.Properties()
                   .ToDictionary(
                        property => GetLogFriendlyName(property.Name),
                        property => GetValue(property.Value.Value<string>() !));
                return AddToParams(paramsList, deviceInfo);
            }

            return false;
        }

        private static bool TryParseTelemetryInfo(
            Dictionary<string, object> paramsList,
            KeyValuePair<string, object> item)
        {
            if (!(item.Value is JObject content))
            {
                return false;
            }

            var isTelemetry = content.Value<string>("message")
              ?.Contains("telemetry", StringComparison.InvariantCultureIgnoreCase);
            if (isTelemetry.GetValueOrDefault())
            {
                var telemetryInfo = content.Properties()
                   .ToDictionary(
                        property => ToTitleCase(property.Name),
                        property => GetValue(property.Value.Value<string>() !));
                return AddToParams(paramsList, telemetryInfo);
            }

            return false;
        }
    }
}
