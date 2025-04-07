using AntiqueBookstore.Data;
using AntiqueBookstore.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AntiqueBookstore.Controllers
{
    [Authorize(Roles = "Manager")]
    public class UserManagementController : Controller
    {
        // Services DI
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserManagementController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        // GET: UserManagement
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Get all users from the database
            var users = await _userManager.Users
                                          .Include(u => u.Employee)
                                          .AsNoTracking()
                                          .ToListAsync();
            
            // Get all employees from the database
            var availableEmployees = await _context.Employees
                                                .Where(e => e.ApplicationUserId == null)
                                                .AsNoTracking()
                                                .OrderBy(e => e.LastName)
                                                .ThenBy(e => e.FirstName)
                                                .ToListAsync();

            // List of unlinked Employees
            ViewData["AvailableEmployees"] = new SelectList(availableEmployees, "Id", "FirstName");


            return View(users);
        }

        // Post: Link employee
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF protection
        public async Task<IActionResult> LinkEmployee(string userId, int selectedEmployeeId)
        {
            if (selectedEmployeeId <= 0)
            {
                // User prompt before returning
                TempData["ErrorMessage"] = "Please select an employee to link.";
                return RedirectToAction(nameof(Index)); 
            }

            // Find the user by ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = $"User with ID {userId} not found.";
                return RedirectToAction(nameof(Index));
            }

            // Find the employee by ID
            var employee = await _context.Employees.FindAsync(selectedEmployeeId);
            if (employee == null)
            {
                TempData["ErrorMessage"] = $"Employee with ID {selectedEmployeeId} not found.";
                return RedirectToAction(nameof(Index));
            }

            // Check if the employee is already linked to another user
            if (employee.ApplicationUserId != null)
            {
                TempData["ErrorMessage"] = $"Employee {employee.FirstName} is already linked to another user.";
                return RedirectToAction(nameof(Index));
            }

            // Link the employee to the user
            try
            {
                // Set link to user
                user.EmployeeId = selectedEmployeeId;
                var userUpdateResult = await _userManager.UpdateAsync(user);

                if (userUpdateResult.Succeeded)
                {
                    // Set link to employee
                    employee.ApplicationUserId = userId;
                    _context.Employees.Update(employee);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"User {user.UserName} successfully linked to Employee {employee.FirstName}.";
                }
                else
                {
                    var errors = string.Join("; ", userUpdateResult.Errors.Select(e => e.Description));
                    TempData["ErrorMessage"] = $"Error linking user: {errors}";
                }
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "An error occurred while saving the link to the database. Please try again.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
            }

            // returning
            return RedirectToAction(nameof(Index));
        }
    }
}
