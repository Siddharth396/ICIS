namespace BusinessLayer.Services.Common
{
    public interface ISqlQueryReader
    {
        string ReadQuery(string scriptName);
    }
}
