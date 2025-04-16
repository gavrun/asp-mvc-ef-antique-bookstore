using AntiqueBookstore.Controllers;
using AntiqueBookstore.Data;
using AntiqueBookstore.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AntiqueBookstore.Models;
using Xunit;
using Moq.EntityFrameworkCore;


namespace AntiqueBookstore.Tests.Controllers
{
    public class EmployeesControllerTests
    {
        // Private fields for moq object
        private readonly Mock<ILogger<EmployeesController>> _mockLogger;

        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;

        private readonly Mock<DbSet<Employee>> _mockEmployeeSet;

        // Controller for tests
        private EmployeesController _controller;

        // Public Ctor
        public EmployeesControllerTests() 
        {
            // ILogger
            _mockLogger = new Mock<ILogger<EmployeesController>>();

            // DbContext DbSet
            var options = new DbContextOptions<ApplicationDbContext>();
            _mockContext = new Mock<ApplicationDbContext>(options);
            _mockEmployeeSet = new Mock<DbSet<Employee>>();

            _mockContext.Setup(c => c.Employees).Returns(_mockEmployeeSet.Object);

            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // UserManager
            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                mockUserStore.Object, null, null, null, null, null, null, null, null);

            // Controller 
            _controller = new EmployeesController(
                _mockContext.Object,
                _mockLogger.Object,
                _mockUserManager.Object // Context, Logger, UserManager
            );

            // Dependencies
            SetupControllerContextAndTempData(_controller);
        }


        // ILogger<EmployeesController> logger = Mock.Of<ILogger<EmployeesController>>();


        // Helper method for setting up implicit dependencies (HttpContext, TempData) for Controller type
        private void SetupControllerContextAndTempData(Controller controller)
        {
            var httpContext = new DefaultHttpContext();

            // User 
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "test-user-id") };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuthType"));

            // TempData 
            var tempDataProvider = new Mock<ITempDataProvider>().Object;
            
            var tempData = new TempDataDictionary(httpContext, tempDataProvider);

            controller.ControllerContext = new ControllerContext() { HttpContext = httpContext };
            controller.TempData = tempData;
        }


        // TESTS


        // Index Action 
        [Fact] 
        public async Task Index_ReturnsViewResult_WithListOfEmployeeIndexViewModels()
        {
            // Arrange 
            var employees = new List<Employee>
            {
                new Employee { Id = 1, FirstName = "John", LastName = "Doe", IsActive = true, ApplicationUserId = "user1" },
                new Employee { Id = 2, FirstName = "Jane", LastName = "Smith", IsActive = false, ApplicationUserId = null }
            }.AsQueryable();

            // Set up DbSet to return valid data [Moq.EntityFrameworkCore]

            // _mockEmployeeSet.As<IDbAsyncEnumerable<Employee>>()
            //      .Setup(m => m.GetAsyncEnumerator())
            //      .Returns(new TestDbAsyncEnumerator<Employee>(employees.GetEnumerator()));
            // _mockEmployeeSet.As<IQueryable<Employee>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Employee>(employees.Provider));

            _mockContext.Setup(x => x.Employees).ReturnsDbSet(employees);

            // Act 
            var result = await _controller.Index();

            // Assert 
            var viewResult = Assert.IsType<ViewResult>(result); // result expected ViewResult

            var model = Assert.IsAssignableFrom<IEnumerable<EmployeeIndexViewModel>>(viewResult.ViewData.Model); // type of Model

            Assert.Equal(2, model.Count()); // count model elements
        }

        // Deactivate Confirmed
        [Fact]
        public async Task DeactivateConfirmed_DeactivatesEmployeeAndUnlinksUser_WhenEmployeeIsActiveAndLinked()
        {
            // Arrange
            var employeeId = 1;
            var userIdToUnlink = "user-to-unlink";
            var testEmployee = new Employee { Id = employeeId, FirstName = "Test", LastName = "Emp", IsActive = true, ApplicationUserId = userIdToUnlink };

            // Set up FindAsync to return Employee

            _mockContext.Setup(c => c.Employees.FindAsync(employeeId)).ReturnsAsync(testEmployee);
            // GetUserAsync, avoid self-deactivation
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser { Id = "different-user-id" }); 

            // Act
            var result = await _controller.DeactivateConfirmed(employeeId);

            // Assert
            // Check Employee changed correctly
            Assert.False(testEmployee.IsActive);
            Assert.Null(testEmployee.ApplicationUserId);

            // Check changes saved
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once); 

            // Check result of controller's action
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName); 

            // 4. Проверяем сообщение в TempData
            Assert.True(_controller.TempData.ContainsKey("SuccessMessage"));
        }

        // Deactivate Confirmed
        [Fact]
        public async Task DeactivateConfirmed_ReturnsRedirect_WithError_WhenAttemptingSelfDeactivation()
        {
            // Arrange
            var employeeId = 1;
            var currentUserId = "current-user-id"; // User ID who is trying to deactivate himself

            var testEmployee = new Employee { Id = employeeId, FirstName = "Self", LastName = "Deactivator", IsActive = true, ApplicationUserId = currentUserId};

            _mockContext.Setup(c => c.Employees.FindAsync(employeeId)).ReturnsAsync(testEmployee);
            // GetUserAsync, user linked
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                            .ReturnsAsync(new ApplicationUser { Id = currentUserId }); 

            // Act
            var result = await _controller.DeactivateConfirmed(employeeId);

            // Assert
            // Check Employee was not changed
            Assert.True(testEmployee.IsActive); 
            Assert.Equal(currentUserId, testEmployee.ApplicationUserId); 

            // Check hanges were not saved 
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never); 

            // Check result
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            // Check TempData
            Assert.True(_controller.TempData.ContainsKey("ErrorMessage"));
            Assert.False(_controller.TempData.ContainsKey("SuccessMessage")); 
        }

    }
}
