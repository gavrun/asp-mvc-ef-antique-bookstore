using AntiqueBookstore.Controllers;
using AntiqueBookstore.Data;
using AntiqueBookstore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiqueBookstore.Tests.Controllers
{
    public class UserManagementControllerTests
    {
        // Private fields for moq object
        private Mock<ILogger<UserManagementController>> _mockLogger;

        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;

        private readonly Mock<DbSet<Employee>> _mockEmployeeSet;
        private readonly Mock<DbSet<ApplicationUser>> _mockUserSet;

        // Controller for tests
        private UserManagementController _controller;


        // Public Ctor
        public UserManagementControllerTests()
        {
            // ILogger
            _mockLogger = new Mock<ILogger<UserManagementController>>();

            // DbContext DbSet
            var options = new DbContextOptions<ApplicationDbContext>();
            _mockContext = new Mock<ApplicationDbContext>(options);
            _mockEmployeeSet = new Mock<DbSet<Employee>>();
            _mockUserSet = new Mock<DbSet<ApplicationUser>>();

            _mockContext.Setup(c => c.Employees).Returns(_mockEmployeeSet.Object);
            _mockContext.Setup(c => c.Users).Returns(_mockUserSet.Object);

            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // UserManager
            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                mockUserStore.Object, null, null, null, null, null, null, null, null);

            // RoleManager
            var mockRoleStore = new Mock<IRoleStore<IdentityRole>>();
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                mockRoleStore.Object, null, null, null, null);

            // Controller 
            //_controller = new UserManagementController()
        }


        // TESTS

    }
}