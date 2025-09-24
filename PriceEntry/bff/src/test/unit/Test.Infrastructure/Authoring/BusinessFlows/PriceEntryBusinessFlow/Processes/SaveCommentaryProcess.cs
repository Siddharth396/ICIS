namespace Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.Processes
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using BusinessLayer.Commentary.DTOs.Input;

    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.GraphQL;

    using Test.Infrastructure.TestData;

    public class SaveCommentaryProcess : IPriceEntryBusinessFlowProcess
    {
        private const string DefaultVersion = "0.1";

        private readonly DateTime assessedDateTime;

        private readonly string commentaryVersion;

        private readonly string contentBlockId;

        private readonly string operationType;

        public SaveCommentaryProcess(string contentBlockId, DateTime assessedDateTime, string? commentaryVersion = null, string operationType = "")
        {
            this.commentaryVersion = commentaryVersion ?? DefaultVersion;
            this.contentBlockId = contentBlockId;
            this.assessedDateTime = assessedDateTime;
            this.operationType = operationType;
        }

        public Task<HttpResponseMessage> Execute(HttpClient httpClient)
        {
            var commentaryGraphQLClient = new CommentaryGraphQLClient(new GraphQLClient(httpClient));

            var commentaryInput = new CommentaryInput
            {
                ContentBlockId = contentBlockId,
                CommentaryId = "92206d33-b512-49a2-941f-f566af6d3468",
                AssessedDateTime = assessedDateTime,
                Version = commentaryVersion,
                OperationType = operationType
            };

            return commentaryGraphQLClient.SaveCommentary(commentaryInput);
        }
    }
}
