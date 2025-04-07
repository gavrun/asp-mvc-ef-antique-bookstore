namespace AntiqueBookstore.Domain.Entities
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
