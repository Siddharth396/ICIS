namespace Infrastructure.SQLDB.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Area")]
    public class RegionCC
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string Description { get; set; } = default!;
    }
}
