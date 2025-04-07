using AntiqueBookstore.Domain.Entities;

namespace AntiqueBookstore.Data.Seed
{
    public static class EmployeeData
    {
        // 
        // Sample data for Employee staff (Manager and Sales)
        // 

        // Employees
        public static IEnumerable<Employee> GetEmployees(ApplicationDbContext context)
        {
            // TODO: Get test employees data

            return Enumerable.Empty<Employee>();
        }
    }
}
