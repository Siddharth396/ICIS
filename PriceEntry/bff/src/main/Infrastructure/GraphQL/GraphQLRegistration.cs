namespace Infrastructure.GraphQL
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    using HotChocolate.Execution.Configuration;
    using HotChocolate.Types;

    using Infrastructure.Logging;

    using Microsoft.Extensions.DependencyInjection;

    [ExcludeFromCodeCoverage]
    public static class GraphQLRegistration
    {
        public static IServiceCollection AddGraphQL(
            this IServiceCollection services,
            Assembly assembly,
            bool isDevMode,
            bool isMutationEnabled,
            bool isTraceEnabled)
        {
            services.AddGraphQLServer()
               .AddFiltering()
               .AddType(new UuidType('D'))
               .AddQueryType(d => d.Name("Query"))
               .AddMutationType(isMutationEnabled)
               .AddTypesFromAssembly(assembly)
               .AddTypeConverter<DateTimeOffset, DateTime>(x => x.UtcDateTime)
               .AddTypeConverter<DateTime, DateTimeOffset>(
                    x => x.Kind is DateTimeKind.Unspecified ? DateTime.SpecifyKind(x, DateTimeKind.Utc) : x)
               .AllowIntrospection(isDevMode)
               .AddAuthorizationHandler<AuthHandler>()
               .AddEventListener(isTraceEnabled)
               .AddErrorFilter<GraphQLErrorFilter>()
               .UseDefaultPipeline()
               .AddDiagnosticEventListener<ErrorLoggingDiagnosticsEventListener>();

            return services;
        }

        private static IRequestExecutorBuilder AddMutationType(
            this IRequestExecutorBuilder builder,
            bool isMutationEnabled)
        {
            if (isMutationEnabled)
            {
                builder.AddMutationType(d => d.Name("Mutation"));
            }

            return builder;
        }

        private static IRequestExecutorBuilder AddEventListener(
            this IRequestExecutorBuilder builder,
            bool isTraceEnabled)
        {
            if (isTraceEnabled)
            {
                builder.AddDiagnosticEventListener<GraphQLLogger>();
            }

            return builder;
        }

        private static IRequestExecutorBuilder AddTypesFromAssembly(this IRequestExecutorBuilder builder, Assembly assembly)
        {
            var queryTypes = from type in assembly.GetTypes()
                             where type.GetCustomAttribute(typeof(AddToGraphQLSchemaAttribute), true) != null
                             select type;

            foreach (var queryType in queryTypes)
            {
                builder.AddType(queryType);
            }

            return builder;
        }
    }
}