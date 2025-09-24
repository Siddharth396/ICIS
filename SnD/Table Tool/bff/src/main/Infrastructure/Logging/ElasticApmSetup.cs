namespace Infrastructure.Logging
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text.RegularExpressions;

    using Infrastructure.Configuration;

    using Elastic.Apm;
    using Elastic.Apm.AspNetCore;
    using Elastic.Apm.DiagnosticSource;
    using Elastic.Apm.SqlClient;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Elastic.Apm.EntityFrameworkCore;

    [ExcludeFromCodeCoverage]
    public static class ElasticApmSetup
    {
        public static IApplicationBuilder UseElasticApm(this IApplicationBuilder builder, IConfiguration configuration)
        {
            var traceEnabled = configuration.IsTraceEnabled();
            if (traceEnabled)
            {
                builder.UseElasticApm(
                    configuration,
                    new HttpDiagnosticsSubscriber(),
                    new EfCoreDiagnosticsSubscriber(),
                    new SqlClientDiagnosticSubscriber());

                MaskUserEmailAddress();
            }

            return builder;

            static void MaskUserEmailAddress()
            {
                var pattern = @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{1}@)";
                Agent.AddFilter(
                    transaction =>
                    {
                        try
                        {
                            if (transaction.Context.User != null
                                && !string.IsNullOrWhiteSpace(transaction.Context.User.Email))
                            {
                                transaction.Context.User.Email = Regex.Replace(
                                    transaction.Context.User.Email,
                                    pattern,
                                    m => new string('*', m.Length));
                            }
                        }
                        catch
                        {
                            // ignored intentionally so that it will not stop the execution in case of an error
                        }

                        return transaction;
                    });
            }
        }
    }
}
