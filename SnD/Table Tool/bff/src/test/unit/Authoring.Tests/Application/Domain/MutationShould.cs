namespace Authoring.Tests.Application.Domain
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;



    using FluentAssertions;


    using MongoDB.Driver;

    using Test.Infrastructure.GraphQL;
    using Test.Infrastructure.Stubs;

    using Xunit;

    [Collection("Authoring stub collection")]
    public class MutationShould
    {
        private readonly QueryExecutor executor;


        public MutationShould(AuthoringStubFixture fixture)
        {
            executor = fixture.Executor;

        }

        [Fact]
        public async Task Save_Only_Content()
        {
            // Arrange
            //var pageId = "page1";

            //// Act
            //await executor.ExecuteAsync(Mutations.Save, ("domainMessage", GetMessageDto("Helllo 11")), ("metaData", GetMetadataDto(pageId)));

            //// Assert
            //var (prePublishedContents, publishedContents, revisionContents) = GetContents(pageId);

            //prePublishedContents.Should().HaveCount(1);
            //publishedContents.Should().HaveCount(0);
            //revisionContents.Should().HaveCount(0);
        }

        [Fact]
        public async Task Save_Content_And_Revisions()
        {
            // Arrange
            //var pageId = "page2";

            //// Act
            //await executor.ExecuteAsync(Mutations.Save, ("domainMessage", GetMessageDto("Helllo 1")), ("metaData", GetMetadataDto(pageId)));
            //await executor.ExecuteAsync(Mutations.Save, ("domainMessage", GetMessageDto("Helllo 2")), ("metaData", GetMetadataDto(pageId)));
            //await executor.ExecuteAsync(Mutations.Save, ("domainMessage", GetMessageDto("Helllo 3")), ("metaData", GetMetadataDto(pageId)));

            //// Assert
            //var (prePublishedContents, publishedContents, revisionContents) = GetContents(pageId);

            //prePublishedContents.Should().HaveCount(1);
            ////publishedContents.Should().HaveCount(0);
            ////revisionContents.Should().HaveCount(2);
        }

        [Fact]
        public async Task Save_Content_And_Publish()
        {
            // Arrange
            //var pageId = "page3";

            //// Act
            //await executor.ExecuteAsync(Mutations.Save, ("domainMessage", GetMessageDto("Helllo 4")), ("metaData", GetMetadataDto(pageId)));
            //await executor.ExecuteAsync(Mutations.Save, ("domainMessage", GetMessageDto("Helllo 5")), ("metaData", GetMetadataDto(pageId)));
            //await executor.ExecuteAsync(Mutations.Save, ("domainMessage", GetMessageDto("Helllo 6")), ("metaData", GetMetadataDto(pageId)));
            //await executor.ExecuteAsync(Mutations.Save, ("domainMessage", GetMessageDto("Helllo 7")), ("metaData", GetMetadataDto(pageId)), ("isPublished", true));

            //// Assert
            //var (prePublishedContents, publishedContents, revisionContents) = GetContents(pageId);

            //prePublishedContents.Should().HaveCount(1);
            //publishedContents.Should().HaveCount(1);
            //revisionContents.Should().HaveCount(3);
            //prePublishedContents[0].Id.Should().Be(publishedContents[0].Id);
            //prePublishedContents[0].Data.Text.Should().Be(publishedContents[0].Data.Text);
        }

        //private (List<ContentDto>, List<ContentDto>, List<ContentDto>) GetContents(string pageId, string mfeIdentifier = "mfeIdentifier1")
        //{
        //    var func = (ContentDto dto) => dto.Metadata.PageId == pageId && dto.Metadata.MfeIdentifier == mfeIdentifier;
        //    var prePublishedContents = mongoDbService.GetCollection<ContentDto>(PrePublishedRepository.CollectionName).AsQueryable().Where(func).ToList();
        //    //var publishedContents = mongoDbService.GetCollection<ContentDto>(PublishedRepository.CollectionName).AsQueryable().Where(func).ToList();
        //    //var revisionContents = mongoDbService.GetCollection<ContentDto>(RevisionRepository.CollectionName).AsQueryable().Where(func).ToList();

        //    return (prePublishedContents, new List<ContentDto>(), new List<ContentDto>());
        //}

        //private DomainMessageDto GetMessageDto(string message)
        //{
        //    return new DomainMessageDto()
        //    {
        //        Text = message,
        //    };
        //}

        //private MetadataDto GetMetadataDto(string pageId)
        //{
        //    return new MetadataDto()
        //    {
        //        PageId = pageId,
        //        MfeIdentifier = "mfeIdentifier1",
        //        WorkflowInstanceId = "test123"
        //    };
        //}
    }
}
