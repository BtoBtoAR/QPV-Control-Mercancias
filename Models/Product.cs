using SQLite;

namespace QPVControlMercancias.Models
{
    [Table("products")]
    public class Product
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed, NotNull]
        public string Barcode { get; set; } = string.Empty;

        [NotNull]
        public string Description { get; set; } = string.Empty;

        [NotNull]
        public string UnitOfMeasure { get; set; } = string.Empty;

        public decimal Cost { get; set; }

        public decimal SellingPrice { get; set; }

        public int Stock { get; set; }

        public string? PhotoPath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
