using AntiqueBookstore.Data;
using AntiqueBookstore.Domain.Entities;
using AntiqueBookstore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AntiqueBookstore.Controllers
{
    [Authorize(Roles = "Manager")] // work in progress
    public class UserManagementController : Controller
    {
        // Services DI
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        

        public UserManagementController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /UserManagement, get all application Users table
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var viewModel = await BuildUserManagementViewModel();

            return View(viewModel);
        }

        // POST: /UserManagement/AssignRole, link new Role or updating existing
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(string userId, int selectedEmployeeId)
        {
            if (string.IsNullOrEmpty(userId) || selectedEmployeeId <= 0)
            {
                TempData["ErrorMessage"] = "Invalid user or employee selected.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            // Load Employee with position history and employee levels
            var selectedEmployee = await GetEmployeeWithLevelDataAsync(selectedEmployeeId);
            if (selectedEmployee == null)
            {
                TempData["ErrorMessage"] = "Selected employee not found.";
                return RedirectToAction(nameof(Index));
            }

            // Check if Employee is active
            if (!selectedEmployee.IsActive)
            {
                TempData["ErrorMessage"] = $"Cannot link user to inactive employee '{selectedEmployee.FirstName} {selectedEmployee.LastName}'.";
                return RedirectToAction(nameof(Index)); // Прерываем операцию
            }

            // Check if Employee is already linked to another user
            if (!string.IsNullOrEmpty(selectedEmployee.ApplicationUserId) && selectedEmployee.ApplicationUserId != userId)
            {
                var linkedUser = await _userManager.FindByIdAsync(selectedEmployee.ApplicationUserId);

                TempData["ErrorMessage"] = $"Employee {selectedEmployee.FirstName} {selectedEmployee.LastName} is already linked to user '{linkedUser?.UserName ?? "another user"}'.";
                return RedirectToAction(nameof(Index));
            }

            // Determining the current employee level
            var currentLevel = GetCurrentLevel(selectedEmployee);
            if (currentLevel == null)
            {
                TempData["ErrorMessage"] = $"Employee {selectedEmployee.FirstName} {selectedEmployee.LastName} does not have a level assigned. Cannot determine role.";
                return RedirectToAction(nameof(Index));
            }

            // Define target role
            string targetRole = GetRoleNameFromLevel(currentLevel.Id);
            if (string.IsNullOrEmpty(targetRole))
            {
                TempData["ErrorMessage"] = $"No corresponding role found for employee level '{currentLevel.Name}'. Cannot assign role.";
                return RedirectToAction(nameof(Index));
            }

            // Check if Role exists in Identity
            if (!await _roleManager.RoleExistsAsync(targetRole))
            {
                TempData["ErrorMessage"] = $"Identity role '{targetRole}' not found in the system.";
                return RedirectToAction(nameof(Index));
            }

            // Unlink Employee from the previous user 
            bool needsSave = await UnlinkOldEmployeeIfExistsAsync(userId, selectedEmployeeId);

            // Manage Identity Roles
            var (roleUpdateSucceeded, roleUpdateError) = await UpdateUserRoleAsync(user, targetRole);
            if (!roleUpdateSucceeded)
            {
                TempData["ErrorMessage"] = roleUpdateError;
                return RedirectToAction(nameof(Index)); // no rollback needed
            }

            // Create new link (needs saving)
            selectedEmployee.ApplicationUserId = userId;
            _context.Update(selectedEmployee);
            needsSave = true;

            if (needsSave)
            {
                try
                {
                    await _context.SaveChangesAsync(); // here we actually save all changes (unlink old, link new)

                    TempData["SuccessMessage"] = $"User '{user.UserName}' successfully linked to employee {selectedEmployee.FirstName} {selectedEmployee.LastName} and assigned role '{targetRole}'.";
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "An error occurred while saving changes.";
                }
            }
            else
            {
                TempData["SuccessMessage"] = $"User '{user.UserName}' linked to employee {selectedEmployee.FirstName} {selectedEmployee.LastName} and assigned role '{targetRole}'.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /UserManagement/RemoveRole, Unlink Employee and delete Roles references
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRole(int employeeId)
        {
            if (employeeId <= 0)
            {
                TempData["ErrorMessage"] = "Invalid employee selected.";
                return RedirectToAction(nameof(Index));
            }

            // Unlink Employee
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                TempData["ErrorMessage"] = "Employee not found.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrEmpty(employee.ApplicationUserId))
            {
                TempData["WarningMessage"] = $"Employee {employee.FirstName} {employee.LastName} is not linked to any user."; 
                return RedirectToAction(nameof(Index)); // already unlinked
            }

            var user = await _userManager.FindByIdAsync(employee.ApplicationUserId);
            if (user == null)
            {
                // corner case: user not found, unlink employee
                employee.ApplicationUserId = null;
                await _context.SaveChangesAsync();

                TempData["WarningMessage"] = $"Linked user for employee {employee.FirstName} {employee.LastName} not found. Link removed."; 
                return RedirectToAction(nameof(Index));
            }

            // BUG: Current User cannot be unlinked
            if (user.Id == _userManager.GetUserId(User))
            {
                TempData["ErrorMessage"] = "You cannot unlink Employee associated with the current account.";
                return RedirectToAction(nameof(Index));
            }

            // Remove Roles references
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    TempData["ErrorMessage"] = $"Failed to remove roles for user '{user.UserName}'. Link not removed. {string.Join(", ", removeResult.Errors.Select(e => e.Description))}";
                    return RedirectToAction(nameof(Index)); // rollback, keep employee linked
                }
            }

            // Remove link
            employee.ApplicationUserId = null;
            _context.Update(employee);
            try
            {
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"User '{user.UserName}' unlinked from employee {employee.FirstName} {employee.LastName} and roles removed (suspended).";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "An error occurred while saving changes.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /UserManagement/SyncRole, Synchronize Role by Employee level
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SyncRole(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Invalid user selected.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            // Load the linked Employee with position history and levels
            var linkedEmployee = await GetLinkedEmployeeWithLevelDataAsync(userId);
            if (linkedEmployee == null)
            {
                TempData["WarningMessage"] = $"User '{user.UserName}' is not linked to any employee. Cannot sync role.";
                return RedirectToAction(nameof(Index));
            }

            // Find out current level
            var currentLevel = GetCurrentLevel(linkedEmployee);
            if (currentLevel == null)
            {
                TempData["ErrorMessage"] = $"Linked employee {linkedEmployee.FirstName} {linkedEmployee.LastName} does not have a level assigned. Cannot determine role."; 
                return RedirectToAction(nameof(Index));
            }

            // Define target role
            string targetRole = GetRoleNameFromLevel(currentLevel.Id);
            if (string.IsNullOrEmpty(targetRole))
            {
                TempData["ErrorMessage"] = $"No corresponding role found for employee level '{currentLevel.Name}'. Cannot sync role.";
                return RedirectToAction(nameof(Index));
            }
            // Check if Role exists in Identity
            if (!await _roleManager.RoleExistsAsync(targetRole))
            {
                TempData["ErrorMessage"] = $"Identity role '{targetRole}' not found in the system.";
                return RedirectToAction(nameof(Index));
            }

            // Unlink Employee
            var existingLink = await _context.Employees.FirstOrDefaultAsync(e => e.ApplicationUserId == userId);
            if (existingLink != null && existingLink.Id != linkedEmployee.Id)
            {
                existingLink.ApplicationUserId = null;
                _context.Update(existingLink);
            }

            // Manage Identity Roles
            var (roleUpdateSucceeded, roleUpdateError) = await UpdateUserRoleAsync(user, targetRole);
            if (!roleUpdateSucceeded)
            {
                TempData["ErrorMessage"] = roleUpdateError; 
            }
            else if (string.IsNullOrEmpty(roleUpdateError)) 
            {
                TempData["SuccessMessage"] = $"Role for user '{user.UserName}' successfully synchronized to '{targetRole}' based on linked {linkedEmployee.FirstName} {linkedEmployee.LastName}.";
            }
            else 
            {
                TempData["InfoMessage"] = roleUpdateError;
            }

            // No need to create a new link, must be already linked
                        
            return RedirectToAction(nameof(Index));
        }

        //
        //
        //
        // REFACTOR: helper method build ViewModel
        private async Task<UserManagementViewModel> BuildUserManagementViewModel()
        {
            var allUsers = await _userManager.Users.ToListAsync();

            // Loading all Employees with history and levels (probably requires optimization)
            var allEmployees = await _context.Employees
                                    .Include(e => e.PositionHistories)
                                    .ThenInclude(ph => ph.Position)
                                    .ThenInclude(p => p.Level)
                                    .ToListAsync();

            // Create a list of UserViewModel
            var userViewModels = new List<UserViewModel>();
            //var employeeDictionary = allEmployees.ToDictionary(e => e.Id);

            foreach (var user in allUsers)
            {
                // Search for a linked Employee in the list
                var linkedEmployee = allEmployees.FirstOrDefault(e => e.ApplicationUserId == user.Id);

                userViewModels.Add(new UserViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = await _userManager.GetRolesAsync(user), // get Identity Roles
                    LinkedEmployeeId = linkedEmployee?.Id,
                    LinkedEmployeeName = linkedEmployee?.FirstName + " " + linkedEmployee?.LastName, // DisplayName = {e?.FirstName} {e?.LastName}
                });
            }

            // List of not linked Employees for modal dialog
            var unlinkedEmployees = allEmployees
                .Where(e => string.IsNullOrEmpty(e.ApplicationUserId))
                .Select(e =>
                {
                    // Get current level to put to the SelectList
                    var currentLevel = GetCurrentLevel(e);
                    var currentLevelName = currentLevel?.Name ?? "N/A";

                    return new { e.Id, DisplayName = $"{e?.FirstName} {e?.LastName} (Level: {currentLevelName})" };
                })
                .OrderBy(e => e.DisplayName)
                .ToList();

            var viewModel = new UserManagementViewModel
            {
                Users = userViewModels.OrderBy(u => u.UserName).ToList(), 
                UnlinkedEmployees = new SelectList(unlinkedEmployees, "Id", "DisplayName")
            };

            return viewModel;
        }

        // helper method for determining Role by level
        private string GetRoleNameFromLevel(int levelId)
        {
            switch (levelId)
            {
                case 1: return "Manager"; // Level 1 = Manager
                case 2: return "Sales";   // Level 2 = Sales
                default: return null;     // No other levels defined in the system
            }
        }

        //
        //
        //
        // REFACTOR: helper method
        private async Task<Employee> GetEmployeeWithLevelDataAsync(int employeeId)
        {
            return await _context.Employees
                .Include(e => e.PositionHistories)
                .ThenInclude(ph => ph.Position)
                .ThenInclude(p => p.Level)
                .FirstOrDefaultAsync(e => e.Id == employeeId);
        }

        // REFACTOR: helper method
        private async Task<Employee> GetLinkedEmployeeWithLevelDataAsync(string userId)
        {
            return await _context.Employees
               .Include(e => e.PositionHistories)
               .ThenInclude(ph => ph.Position)
               .ThenInclude(p => p.Level)
               .FirstOrDefaultAsync(e => e.ApplicationUserId == userId);
        }

        // REFACTOR: helper method
        private Level GetCurrentLevel(Employee employee)
        {
            if (employee?.PositionHistories == null || !employee.PositionHistories.Any())
            {
                return null;
            }

            var currentPositionHistory = employee.PositionHistories
                                            .OrderByDescending(ph => ph.StartDate)
                                            .FirstOrDefault();

            return currentPositionHistory?.Position?.Level;
        }

        // REFACTOR: helper method
        private async Task<(bool Succeeded, string Message)> UpdateUserRoleAsync(ApplicationUser user, string targetRole)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);

            // do nothing if Role is set
            if (currentRoles.Count == 1 && currentRoles.First() == targetRole)
            {
                return (true, $"User '{user.UserName}' already has the correct role '{targetRole}'. No changes made."); // true InfoMessage tuple
            }

            // remove current Roles
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    return (false, $"Failed to remove current roles for user '{user.UserName}'. {string.Join(", ", removeResult.Errors.Select(e => e.Description))}");
                }
            }

            // set target Role
            var addResult = await _userManager.AddToRoleAsync(user, targetRole);
            if (!addResult.Succeeded)
            {
                // try and stay on a safe side (best effort)
                await _userManager.AddToRolesAsync(user, currentRoles);

                return (false, $"Failed to add role '{targetRole}' for user '{user.UserName}'. {string.Join(", ", addResult.Errors.Select(e => e.Description))}"); // false ErrorMessage tuple
            }

            return (true, null); // true no Message tuple
        }

        // REFACTOR: helper method
        private async Task<bool> UnlinkOldEmployeeIfExistsAsync(string userId, int currentSelectedEmployeeId)
        {
            var existingLink = await _context.Employees.FirstOrDefaultAsync(e => e.ApplicationUserId == userId);
            if (existingLink != null && existingLink.Id != currentSelectedEmployeeId)
            {
                // was linked, only mark for update
                existingLink.ApplicationUserId = null;
                _context.Update(existingLink);

                return true; // true need saving
            }
            return false; // false does not need saving
        }

    }
}
