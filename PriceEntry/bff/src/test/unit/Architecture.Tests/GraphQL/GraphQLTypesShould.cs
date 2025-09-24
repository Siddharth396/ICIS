namespace Architecture.Tests.GraphQL
{
    using Architecture.Tests.NetArchTestsCustomization;

    using Authoring.Application.PriceDisplay.ContentBlock.Mutation;
    using Authoring.Application.PriceDisplay.ContentBlock.Query;
    using Authoring.Application.PriceDisplay.PriceSeriesSelection;
    using Authoring.Application.PriceEntry.Mutation;

    using HotChocolate.Types;

    using Infrastructure.GraphQL;

    using NetArchTest.Rules;

    using Xunit;

    public sealed class GraphQLTypesShould
    {
        [Fact]
        public void Follow_A_Naming_Convention()
        {
            Types
               .InAssemblies([SolutionAssemblies.AuthoringBff, SolutionAssemblies.SubscriberBff])
               .That()
               .HaveCustomAttribute(typeof(ExtendObjectTypeAttribute))
               .And()
               //// The following are exceptions to the rule and can be refactored later.
               //// The main goal is to have a consistent naming convention from now on.
               .DoNotHaveName(
                    nameof(SavePriceEntryData),
                    nameof(PriceSeriesSelectionQueriesForDisplay),
                    nameof(ContentBlockQueriesForDisplay),
                    nameof(ContentBlockMutationsForDisplay))
               .Should()
               .HaveNameEndingWith("Queries")
               .Or()
               .HaveNameEndingWith("Mutations")
               .Or()
               .HaveNameEndingWith("Resolvers")
               .Or()
               .HaveNameEndingWith("Resolver")
               .GetResult()
               .ShouldBeSuccessful();
        }

        [Fact]
        public void Have_The_Required_Attributes()
        {
            Types.InAssemblies([SolutionAssemblies.AuthoringBff, SolutionAssemblies.SubscriberBff])
               .That()
               .HaveNameEndingWith("Queries")
               .Or()
               .HaveNameEndingWith("Mutations")
               .Or()
               .HaveNameEndingWith("Resolvers")
               .Or()
               .HaveNameEndingWith("Resolver")
               .Or()
               .HaveName(
                    nameof(SavePriceEntryData),
                    nameof(PriceSeriesSelectionQueriesForDisplay),
                    nameof(ContentBlockQueriesForDisplay),
                    nameof(ContentBlockMutationsForDisplay))
               .Should()
               .HaveCustomAttribute(typeof(ExtendObjectTypeAttribute))
               .And()
               .HaveCustomAttribute(typeof(AddToGraphQLSchemaAttribute))
               .GetResult()
               .ShouldBeSuccessful();
        }

        /// <summary>
        /// Read more about constructor injection here:
        /// https://chillicream.com/docs/hotchocolate/v13/server/dependency-injection/#constructor-injection
        /// </summary>
        [Fact]
        public void Have_Only_Default_Constructor()
        {
            Types.InAssemblies([SolutionAssemblies.AuthoringBff, SolutionAssemblies.SubscriberBff])
               .That()
               .HaveCustomAttribute(typeof(ExtendObjectTypeAttribute))
               .Should()
               .MeetCustomRule(DefaultConstructorsOnly.Rule)
               .GetResult()
               .ShouldBeSuccessful();
        }
    }
}
