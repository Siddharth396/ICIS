namespace Infrastructure.SQLDB.Models
{
    using System;

    public class TableConfiguration
    {
        public int Id { get; set; }
        public string ContentBlockId { get; set; } = default!;
        public int MinorVersion { get; set; }
        public int MajorVersion { get; set; }
        public string Filter { get; set; } = default!;
        public DateTime CreatedOn { get; set; }
    }
}
