using AntiqueBookstore.Domain.Entities;

namespace AntiqueBookstore.Data.Seed
{
    public static class CustomerData
    {
        // 
        // Sample data for Customers base
        // 

        // Customers
        public static IEnumerable<Customer> GetCustomers()
        {
            // TODO: Get test customers data

            return Enumerable.Empty<Customer>();
        }

        // Delivery Addresses
        public static IEnumerable<DeliveryAddress> GetDeliveryAddresses(ApplicationDbContext context)
        {
            // TODO: Get test delivery addresses data

            return Enumerable.Empty<DeliveryAddress>();
        }
    }
}
