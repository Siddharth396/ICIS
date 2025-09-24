namespace Infrastructure.SQLDB.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Infrastructure.SQLDB.Extensions;

    public class CapacityDevelopment
    {
        private string country;
        public string Country 
        {
            get => country.TransformCountry();
            set => country = value;
        }
        public string Company { get; set; }
        public string Site { get; set; }
        public int PlantNo { get; set; }
        public string Type { get; set; }
        public string EstimatedStart { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal NewAnnualCapacity { get; set; }
        [Column(TypeName = "decimal(7,2)")]
        public decimal CapacityChange { get; set; }
        public string PercentChange { get; set; }
        public string LastUpdated { get; set; }
    }
}
