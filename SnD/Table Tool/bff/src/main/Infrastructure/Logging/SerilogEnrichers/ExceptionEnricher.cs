namespace Infrastructure.Logging.SerilogEnrichers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Serilog.Core;
    using Serilog.Events;

    [ExcludeFromCodeCoverage]
    public class ExceptionEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Exception == null)
            {
                return;
            }

            var exceptionProperty = propertyFactory.CreateProperty(
                "ErrorMessage",
                string.Join(". ", GetExceptionMessages(logEvent.Exception)));
            var stackTraceProperty = propertyFactory.CreateProperty(
                "StackTrace",
                logEvent.Exception.StackTrace?.Replace(Environment.NewLine, "\\r\\n"));
            logEvent.AddOrUpdateProperty(exceptionProperty);
            logEvent.AddOrUpdateProperty(stackTraceProperty);
        }

        private IEnumerable<string> GetExceptionMessages(Exception? ex)
        {
            if (ex == null)
            {
                yield break;
            }

            var innerException = ex;
            do
            {
                yield return innerException.Message.Replace(Environment.NewLine, "\\r\\n");
                innerException = innerException.InnerException;
            }
            while (innerException != null);
        }
    }
}