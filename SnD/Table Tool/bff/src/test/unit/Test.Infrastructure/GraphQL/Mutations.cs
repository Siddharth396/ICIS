namespace Test.Infrastructure.GraphQL
{
    public static class Mutations
    {
        public const string Save = @"mutation save($domainMessage:DomainMessageDtoInput!, $metaData: MetadataDtoInput!, $isPublished: Boolean){
              save(domainMessage:$domainMessage, metadata: $metaData, isPublished: $isPublished)
            }";
    }
}
