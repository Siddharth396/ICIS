namespace Test.Infrastructure.GraphQL
{
    using System.Collections.Generic;
    using System.Text.Json;

    public class GraphQLQueryBuilder
    {
        private readonly Dictionary<string, object?> variables = new();

        private string query = string.Empty;

        public static GraphQLQueryBuilder New()
        {
            return new GraphQLQueryBuilder();
        }

        public GraphQLQueryBuilder AddVariable(string key, object? value)
        {
            variables.Add(
                key,
                value);
            return this;
        }

        public GraphQLQueryBuilder AddVariables(params (string key, object value)[] tuples)
        {
            foreach (var (key, value) in tuples)
            {
                variables.Add(
                    key,
                    value);
            }

            return this;
        }

        public string Build()
        {
            var payload = new { Query = query, Variables = variables };

            return JsonSerializer.Serialize(
                payload,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        public GraphQLQueryBuilder SetQuery(string q)
        {
            query = q;
            return this;
        }
    }
}