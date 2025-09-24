namespace Test.Infrastructure.Extensions
{
    using WireMock.Matchers;
    using WireMock.RequestBuilders;

    public static class RequestBuilderExtensions
    {
        public static IRequestBuilder WithParams(this IRequestBuilder requestBuilder, params (string Name, string Value)[] parameters)
        {
            foreach (var parameter in parameters)
            {
                requestBuilder = requestBuilder.WithParam(parameter.Name, new ExactMatcher(parameter.Value));
            }

            return requestBuilder;
        }
    }
}