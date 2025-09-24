namespace Infrastructure.SQLDB.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string Description { get; set; } = default!;
    }
}
