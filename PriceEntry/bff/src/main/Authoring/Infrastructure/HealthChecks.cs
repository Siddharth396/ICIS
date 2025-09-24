namespace Authoring.Infrastructure
{
    using System;

    using global::Infrastructure.Services.CanvasApi;
    using global::Infrastructure.Services.OAuth;
    using global::Infrastructure.Services.PeriodGenerator;
    using global::Infrastructure.Services.Workflow;

    using Microsoft.Extensions.DependencyInjection;

    public static class HealthChecks
    {
        public static Action<IHealthChecksBuilder> RegisterHealthChecks =>
            builder => builder
               .AddPeriodGeneratorServiceCheck()
               .AddOAuthSystemCheck()
               .AddWorkflowSystemCheck()
               .AddCanvasApiServiceCheck();
    }
}
