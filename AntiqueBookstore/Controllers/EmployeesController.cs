using AntiqueBookstore.Data;
using AntiqueBookstore.Domain.Entities;
using AntiqueBookstore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AntiqueBookstore.Controllers
{
    // [Authorize(Roles = "Manager")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmployeesController> _logger;

        private readonly UserManager<ApplicationUser> _userManager;


        public EmployeesController(ApplicationDbContext context, ILogger<EmployeesController> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }


        // GET: Employees
        // show list of employees
        public async Task<IActionResult> Index()
        {
            // get a list of employees with the current position
            var employeesData = await _context.Employees
                .Include(e => e.PositionHistories)
                .ThenInclude(ph => ph.Position)
                .ThenInclude(p => p.Level)
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ToListAsync();

            var viewModel = employeesData.Select(e =>
            {
                // current position the last entry in history without end date
                var currentPositionHistory = e.PositionHistories?
                                              .OrderByDescending(ph => ph.StartDate)
                                              .FirstOrDefault(ph => ph.EndDate == null);
                
                return new EmployeeIndexViewModel
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    CurrentPositionTitle = currentPositionHistory?.Position?.Title, // protect null
                    CurrentLevelName = currentPositionHistory?.Position?.Level?.Name,
                    IsActive = e.IsActive
                };
            }).ToList();

            // add info table Levels
            var levelsData = await _context.Levels
                                   .OrderBy(l => l.Name)
                                   .ToListAsync();

            ViewData["LevelsList"] = levelsData;

            return View(viewModel); 
        }

        // GET: Employees/Details/5
        // show employee details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // TODO: get an employee by ID with a history of positions and a related user

            var employee = await _context.Employees
                .Include(e => e.PositionHistories)
                .ThenInclude(ph => ph.Position)
                .ThenInclude(p => p.Level)
                // include ApplicationUser
                //.Include(e => e.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            var currentPositionHistory = employee.PositionHistories?
                                     .OrderByDescending(ph => ph.StartDate)
                                     .FirstOrDefault(ph => ph.EndDate == null);

            var viewModel = new EmployeeDetailsViewModel
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                HireDate = employee.HireDate,
                IsActive = employee.IsActive,
                CurrentPositionTitle = currentPositionHistory?.Position?.Title,
                CurrentLevelName = currentPositionHistory?.Position?.Level?.Name,
                // ApplicationUserId = employee.ApplicationUserId,
                // UserEmail = employee.ApplicationUser?.Email,
                PositionHistory = employee.PositionHistories?
                // from earlier to later
                .OrderBy(ph => ph.StartDate) 
                // project each PositionHistory entry into PositionHistoryViewModel
                .Select(ph => new PositionHistoryViewModel
                {
                    PositionTitle = ph.Position?.Title ?? "N/A", // protect from null
                    LevelName = ph.Position?.Level?.Name ?? "N/A",
                    StartDate = ph.StartDate,
                    EndDate = ph.EndDate
                }).ToList() ?? new List<PositionHistoryViewModel>() // If there is no history, create an empty List
            };

            return View(viewModel);
        }

        // GET: Employees/Create
        // display the employee creation form
        public async Task<IActionResult> Create()
        {
            // TODO: create ViewModel link to Positions

            var positionsList = await _context.Positions
                                      .OrderBy(p => p.Title)
                                      .Select(p => new { p.Id, p.Title })
                                      .ToListAsync();

            var viewModel = new EmployeeCreateViewModel
            {
                Positions = new SelectList(positionsList, "Id", "Title"), // SelectList collection to view

                HireDate = DateTime.Today // force set the HireDate to view
            };

            return View(viewModel);
        }

        // POST: Employees/Create
        // process form data to create an employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeCreateViewModel viewModel)
        {
            // TODO: save Employee and PositionHistory record

            if (ModelState.IsValid)
            {
                var newEmployee = new Employee
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    HireDate = viewModel.HireDate,
                    IsActive = true // IsActive New employee 
                                    // ApplicationUserId User requires activation
                };

                var initialPositionHistory = new PositionHistory
                {
                    Employee = newEmployee, 
                    PositionId = viewModel.SelectedPositionId,
                    StartDate = viewModel.HireDate, 
                    EndDate = null // The active and current position has no end date by definition
                };

                _context.Add(newEmployee);
                _context.Add(initialPositionHistory);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Employee {newEmployee.FirstName} {newEmployee.LastName} created successfully.";

                return RedirectToAction(nameof(Index));
            }

            // TODO: get ViewModel on validation error
            var positionsList = await _context.Positions
                                      .OrderBy(p => p.Title)
                                      .Select(p => new { p.Id, p.Title })
                                      .ToListAsync();

            viewModel.Positions = new SelectList(positionsList, "Id", "Title", viewModel.SelectedPositionId);

            return View(viewModel);
        }

        // GET: Employees/Edit/5
        // display the employee edit form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // TODO: get an employee and his current position for editing
            var employee = await _context.Employees
                .Include(e => e.PositionHistories)
                //.Include(e => e.ApplicationUser)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            // check if the employee is active
            if (!employee.IsActive)
            {
                TempData["WarningMessage"] = $"Employee {employee.FirstName} {employee.LastName} is inactive and cannot be edited.";
                return RedirectToAction(nameof(Index));
            }

            var currentPositionHistory = employee.PositionHistories?
                                     .OrderByDescending(ph => ph.StartDate)
                                     .FirstOrDefault(ph => ph.EndDate == null);

            if (currentPositionHistory == null)
            {
                TempData["ErrorMessage"] = $"Employee {employee.FirstName} {employee.LastName} does not have a current position assigned.";
                return RedirectToAction(nameof(Index));
            }

            var positionsList = await _context.Positions
                .OrderBy(p => p.Title)
                .Select(p => new { p.Id, p.Title })
                .ToListAsync();

            //var usersList = await _userManager.Users
            //    .OrderBy(u => u.Email)
            //    .Select(u => new { u.Id, u.Email })
            //    .ToListAsync();

            // TODO: ViewModel
            var viewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                HireDate = employee.HireDate,
                IsActive = employee.IsActive, 
                Comment = employee.Comment,
                SelectedPositionId = currentPositionHistory.PositionId,
                Positions = new SelectList(positionsList, "Id", "Title", currentPositionHistory.PositionId),
                //SelectedApplicationUserId = employee.ApplicationUserId,
                //CurrentUserEmail = employee.ApplicationUser?.Email,
                //ApplicationUsers = new SelectList(usersList, "Id", "Email", employee.ApplicationUserId)
            };

            return View(viewModel);
        }

        // POST: Employees/Edit/5
        // process form data for editing an employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeEditViewModel viewModel)
        {
            // TODO: check employee 
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            // Reload lists for the case of a validation error BEFORE checking ModelState,
            // which ensures that the form will be correctly displayed with errors
            Func<Task> reloadDropdowns = async () => {
                var positionsList = await _context.Positions
                    .OrderBy(p => p.Title)
                    .Select(p => new { p.Id, p.Title })
                    .ToListAsync();
                //var usersList = await _userManager.Users
                //    .OrderBy(u => u.Email)
                //    .Select(u => new { u.Id, u.Email })
                //    .ToListAsync();
                viewModel.Positions = new SelectList(positionsList, "PositionId", "Title", viewModel.SelectedPositionId);
                //viewModel.ApplicationUsers = new SelectList(usersList, "Id", "Email", viewModel.SelectedApplicationUserId);
                //var linkedUser = usersList
                //    .FirstOrDefault(u => u.Id == viewModel.SelectedApplicationUserId);
                //viewModel.CurrentUserEmail = linkedUser?.Email;
            };

            var employeeToUpdate = await _context.Employees
                    .Include(e => e.PositionHistories)
                    // ApplicationUser not required, because we are just updating the ID
                    .FirstOrDefaultAsync(e => e.Id == id);

            if (employeeToUpdate == null)
            {
                return NotFound();
            }

            // check if the employee is active
            if (!employeeToUpdate.IsActive)
            {
                TempData["WarningMessage"] = $"Employee {employeeToUpdate.FirstName} {employeeToUpdate.LastName} is inactive and cannot be edited.";
                return RedirectToAction(nameof(Index));
            }

            // TODO: update Employee and managing PositionHistory

            if (ModelState.IsValid)
            {
                // employee loaded and verified

                var currentPositionHistory = employeeToUpdate.PositionHistories
                                                 .OrderByDescending(ph => ph.StartDate)
                                                 .FirstOrDefault(ph => ph.EndDate == null);

                if (currentPositionHistory == null)
                {
                    TempData["ErrorMessage"] = "Could not find the current position record to update. Please try again.";
                    await reloadDropdowns(); // Reload lists before returning View
                    
                    return View(viewModel);
                }

                // update employee properties
                employeeToUpdate.FirstName = viewModel.FirstName;
                employeeToUpdate.LastName = viewModel.LastName;
                employeeToUpdate.Comment = viewModel.Comment;
                // BLOCKED: update ApplicationUserId
                // If SelectedApplicationUserId is an empty string, set it to Null, otherwise set it to Value
                //employeeToUpdate.ApplicationUserId = string.IsNullOrEmpty(viewModel.SelectedApplicationUserId) ? null : viewModel.SelectedApplicationUserId;

                // update employee position
                bool positionChanged = currentPositionHistory.PositionId != viewModel.SelectedPositionId;

                if (positionChanged)
                {
                    var changeDate = DateTime.UtcNow;
                    currentPositionHistory.EndDate = changeDate;

                    _context.Update(currentPositionHistory);

                    var newPositionHistory = new PositionHistory
                    {
                        EmployeeId = employeeToUpdate.Id,
                        PositionId = viewModel.SelectedPositionId,
                        StartDate = changeDate,
                        EndDate = null
                    };

                    _context.Add(newPositionHistory);
                }

                try
                {
                    _context.Update(employeeToUpdate);

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Employee {employeeToUpdate.FirstName} {employeeToUpdate.LastName} updated successfully.";

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException dbEx) 
                {
                    _logger.LogError(dbEx, "Concurrency error updating book with id {EmployeeId}.", id);

                    // potential conflicts, checking if record deleted while we were editing it
                    var exists = await EmployeeExists(viewModel.Id);
                    if (!exists)
                    {
                        TempData["ErrorMessage"] = "The employee you were editing was deleted by another user.";
                        return NotFound();
                    }
                    else
                    {
                        // record still exists but has been modified
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                            + "was modified by another user after you got the original value. "
                            + "Edit operation was canceled. If you still want to edit this record, "
                            + "click the 'Cancel' button to reload the page.");
                    }
                }
                catch (Exception ex) 
                {
                    _logger.LogError(ex, "Error updating employee with ID {EmployeeId}", viewModel.Id);
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred while saving changes. Please try again.");
                }
            }

            // TODO: get ViewModel on validation error
            await reloadDropdowns();

            //viewModel contains user input and validation errors
            return View(viewModel);
            //return RedirectToAction(nameof(Index)); 
        }

        // GET: Employees/Deactivate/5
        // display confirmation of employee deactivation
        public async Task<IActionResult> Deactivate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // TODO: get employee for deactivation confirmation
            var employee = await _context.Employees
                                 .FirstOrDefaultAsync(m => m.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            if (!employee.IsActive)
            {
                TempData["WarningMessage"] = $"Employee {employee.FirstName} {employee.LastName} is already inactive.";
                return RedirectToAction(nameof(Index));
            }

            return View(employee); 
        }

        // POST: Employees/Deactivate/5
        // deactivate employee
        [HttpPost, ActionName("Deactivate")] // ActionName GET
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateConfirmed(int id)
        {
            // TODO: deactivate (IsActive = false)
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }
            
            if (employee.IsActive)
            {
                employee.IsActive = false;
                _context.Update(employee); // update IsActive

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Employee {employee.FirstName} {employee.LastName} has been deactivated.";
            }
            else
            {
                TempData["WarningMessage"] = $"Employee {employee.FirstName} {employee.LastName} is already inactive.";
                return RedirectToAction(nameof(Index));
            }

            //return View(employee);
            return RedirectToAction(nameof(Index)); // Post-Redirect-Get (PRG)
        }

        // helper method (Edit POST)
        private async Task<bool> EmployeeExists(int id)
        {
            return await _context.Employees.AnyAsync(e => e.Id == id);
        }

        // GET: Employees/Activate/5
        // display confirmation of employee activation
        public async Task<IActionResult> Activate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // find employee for confirmation
            var employee = await _context.Employees
                                         .FirstOrDefaultAsync(m => m.Id == id);
           
            if (employee == null)
            {
                return NotFound();
            }

            // check if the employee is inactive 
            if (employee.IsActive)
            {
                TempData["WarningMessage"] = $"Employee {employee.FirstName} {employee.LastName} is already active.";
                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }

        // POST: Employees/Activate/5
        // activate employee 
        [HttpPost, ActionName("Activate")] // ActionName to GET
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            if (!employee.IsActive) // (IsActive = true)
            {
                employee.IsActive = true;
                _context.Update(employee);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Employee {employee.FirstName} {employee.LastName} has been activated.";
            }
            else
            {
                TempData["InfoMessage"] = $"Employee {employee.FirstName} {employee.LastName} was already active.";
            }

            return RedirectToAction(nameof(Index)); // Post-Redirect-Get (PRG)
        }

    }
}
