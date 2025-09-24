namespace Infrastructure.SQLDB.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Infrastructure.SQLDB.Extensions;

    public class Outage
    {
        private string country;

        public string OutageStart {  get; set; }
        public string OutageEnd { get; set; }
        public string Country
        {
            get => country.TransformCountry();
            set => country = value;
        }
        public string Company { get; set; }
        public string Site { get; set; }
        public int PlantNo { get; set; }
        public string Cause { get; set; }
        public string CapacityLoss { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal TotalAnnualCapacity { get; set; }
        public string LastUpdated { get; set; }
        public string Comments { get; set; }
    }
}
