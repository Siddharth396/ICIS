namespace BusinessLayer.DataPackage.Models
{
    public record DataPackageId(string Value)
    {
        public override string ToString()
        {
            return Value;
        }
    }
}
