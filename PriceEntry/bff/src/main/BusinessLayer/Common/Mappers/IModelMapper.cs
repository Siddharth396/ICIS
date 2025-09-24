namespace BusinessLayer.Common.Mappers
{
    public interface IModelMapper<in TMongoModel, out TGraphQLModel>
    {
        TGraphQLModel Map(TMongoModel model);
    }
}
