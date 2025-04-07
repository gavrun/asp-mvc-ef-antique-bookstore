using AntiqueBookstore.Domain.Entities;

namespace AntiqueBookstore.Data.Seed
{
    public static class OrderData
    {
        // 
        // Sample data for Customers Orders
        // 

        // Orders
        public static IEnumerable<Order> GetOrders(ApplicationDbContext context)
        {
            // TODO: Get test orders data

            return Enumerable.Empty<Order>();
        }

        // Sales items
        public static IEnumerable<Sale> GetSales(ApplicationDbContext context)
        {
            // TODO: Get test sales data

            return Enumerable.Empty<Sale>();
        }

        // Sale Events
        public static IEnumerable<SaleEvent> GetSaleEvents(ApplicationDbContext context)
        {
            // TODO: Get test sale events data

            return Enumerable.Empty<SaleEvent>();
        }
    }
}
