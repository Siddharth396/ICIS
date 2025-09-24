namespace Test.Infrastructure.GraphQL
{
    public static class Queries
    {
        public const string GetVersions = @"query versions($pageId:String!, $mfeIdentifier: String!){
            versions(pageId:$pageId, mfeIdentifier: $mfeIdentifier) {
              id
              createdOn
              createdBy
              modifiedOn
              modifiedBy
              version
              metadata {
                pageId
                workflowInstanceId
                mfeIdentifier
              }
              data {
                text
              }
            }
          }";

        public const string SpecifcVersion = @"query specifcVersion($pageId:String!, $mfeIdentifier: String!, $version: Int!){
            specifcVersion(pageId:$pageId, mfeIdentifier: $mfeIdentifier, version: $version) {
              id
              createdOn
              createdBy
              modifiedOn
              modifiedBy
              version
              metadata {
                pageId
                workflowInstanceId
                mfeIdentifier
              }
              data {
                text
              }
            }
          }";

        public const string PublishedVersion = @"query publishedVersion($pageId:String!, $mfeIdentifier: String!){
            publishedVersion(pageId:$pageId, mfeIdentifier: $mfeIdentifier) {
              id
              createdOn
              createdBy
              modifiedOn
              modifiedBy
              version
              metadata {
                pageId
                workflowInstanceId
                mfeIdentifier
              }
              data {
                text
              }
            }
          }";

        public const string Get = @"query get($pageId:String!, $mfeIdentifier: String!){
            get(pageId:$pageId, mfeIdentifier: $mfeIdentifier) {
              id
              createdOn
              createdBy
              modifiedOn
              modifiedBy
              version
              metadata {
                pageId
                workflowInstanceId
                mfeIdentifier
              }
              data {
                text
              }
            }
          }";

        public const string GetProducts = @"query query {
          products {
            code
            description
            id
          }
        }";
        public const string GetRegions = @"query query {
          regions {
            code
            description
            id
          }
        }";
    }
}
