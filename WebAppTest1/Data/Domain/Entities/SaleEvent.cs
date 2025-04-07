using System.ComponentModel.DataAnnotations.Schema;

namespace AntiqueBookstore.Domain.Entities
{
    public class SaleEvent
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(4, 2)")] 
        public decimal Discount { get; set; } // Discount, for example, 0.15 for 15% discount (percent)
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Navigation property to Sale
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
