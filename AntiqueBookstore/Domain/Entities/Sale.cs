using System.ComponentModel.DataAnnotations.Schema;

namespace AntiqueBookstore.Domain.Entities
{
    public class Sale
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(9, 2)")]
        public decimal SalePrice { get; set; } // Price specific to the Sale

        // Foreign key to Order
        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!; // Required

        // Foreign key to Books
        public int BookId { get; set; }
        public virtual Book Book { get; set; } = null!; // Required

        // Foreign key to SaleEvent
        public int? EventId { get; set; } // Nullable, if the Sale is not part of a special event
        public virtual SaleEvent? SaleEvent { get; set; }
    }
}
