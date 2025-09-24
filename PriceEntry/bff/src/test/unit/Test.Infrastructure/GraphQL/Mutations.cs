namespace Test.Infrastructure.GraphQL
{
    public static class Mutations
    {
        public const string SavePriceSeriesItem = @"mutation updatePriceEntryGridData($priceItemInput:PriceItemInput!){
          updatePriceEntryGridData(priceItemInput:$priceItemInput){
              id
            }
        }";

        public const string SaveContentBlock = @"mutation saveContentBlock($contentBlockInput:ContentBlockInput!){
          saveContentBlock(contentBlockInput:$contentBlockInput){
              contentBlockId
              version
              errorCodes
            }
        }";

        public const string SaveCommentary = @"mutation saveCommentary($commentaryInput:CommentaryInput!){
          saveCommentary(commentaryInput:$commentaryInput){
              contentBlockId
              commentaryId
              version
              assessedDateTime
            }
        }";

        public const string SaveUserPreference =
            @"mutation saveUserPreference($userPreferenceInput:UserPreferenceInput!){
          saveUserPreference(userPreferenceInput:$userPreferenceInput){
              id
            }
        }";

        public const string InitiateCorrectionForDataPackage =
            @"mutation initiateCorrectionForDataPackage($contentBlockId: String!, $version: Int!, $assessedDateTime: DateTime!, $reviewPageUrl: String!){
                initiateCorrectionForDataPackage(contentBlockId:$contentBlockId, version:$version, assessedDateTime:$assessedDateTime, reviewPageUrl:$reviewPageUrl)   
            }";

        public const string DataPackageTransitionToState =
            @"mutation dataPackageTransitionToState($contentBlockId: String!, $version: Int!, $assessedDateTime: DateTime!, $nextState: String!, $operationType: String!, $reviewPageUrl: String!) {
                dataPackageTransitionToState(contentBlockId: $contentBlockId, version: $version, assessedDateTime: $assessedDateTime, nextState: $nextState, operationType:$operationType, reviewPageUrl:$reviewPageUrl) {
                  isSuccess
                  errorCode
                }
            }";
    }
}