using AntiqueBookstore.Data;
using AntiqueBookstore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AntiqueBookstore.Controllers
{
    // [Authorize(Roles = "Manager")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;


        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
            //_userManager = userManager;
        }


        //public IActionResult Index()
        //{
        //    return View();
        //}


        // GET: Employees
        // show list of employees
        public async Task<IActionResult> Index()
        {
            // TODO: get a list of employees with the current position

            return View(new List<Employee>()); // stub
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

            // stub
            var employeeStub = new Employee { Id = id.Value, FirstName = "Stub", LastName = "Employee" };

            return View(employeeStub); // stub
        }

        // GET: Employees/Create
        // display the employee creation form
        public IActionResult Create()
        {
            // TODO: create ViewModel link to Positions
            // ViewBag.PositionId = new SelectList(_context.Positions, "PositionId", "Title");

            return View(); // stub ViewModel
        }

        // POST: Employees/Create
        // process form data to create an employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id) // remove id, EmployeeViewModel viewModel 
        {
            // TODO: save Employee and PositionHistory record

            // if (ModelState.IsValid)
            // {
            //     // Employee
            //     // PositionHistory
            //     // save to DB
            //     await _context.SaveChangesAsync();
            //     return RedirectToAction(nameof(Index));
            // }

            // TODO: get ViewModel on validation error
            // return View(viewModel);

            return RedirectToAction(nameof(Index)); // stub
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
            // TODO: ViewModel

            // stub
            var employeeStub = new Employee { Id = id.Value, FirstName = "Stub", LastName = "Employee" };
            // ViewBag.PositionId = new SelectList(_context.Positions, "PositionId", "Title", currentPositionId);

            return View(employeeStub); // stub (ViewModel)
        }

        // POST: Employees/Edit/5
        // process form data for editing an employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id) // EmployeeViewModel viewModel
        {
            // TODO: check id == viewModel.EmployeeId
            // if (id != viewModel.EmployeeId) { return NotFound(); }

            // TODO: update Employee and managing PositionHistory

            // if (ModelState.IsValid)
            // {
            //     try
            //     {
            //         // Employee
            //         // Employee properties
            //         // check on change
            //         // update/add PositionHistory
            //         await _context.SaveChangesAsync();
            //     }
            //     catch (DbUpdateConcurrencyException) {  }
            //     return RedirectToAction(nameof(Index));
            // }

            // TODO: get ViewModel on validation error
            // return View(viewModel);

            return RedirectToAction(nameof(Index)); // stub
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

            // stub
            var employeeStub = new Employee { Id = id.Value, FirstName = "Stub", LastName = "Employee" };

            return View(employeeStub); // stub
        }

        // POST: Employees/Deactivate/5
        // deactivate employee
        [HttpPost, ActionName("Deactivate")] // ActionName GET
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateConfirmed(int id)
        {
            // TODO: deactivate (IsActive = false)

            // var employee = await _context.Employees.FindAsync(id);
            // if (employee != null)
            // {
            //     employee.IsActive = false;
            //     _context.Update(employee);
            //     await _context.SaveChangesAsync();
            // }

            return RedirectToAction(nameof(Index)); // stub
        }

        // helper method (Edit POST)
        // private bool EmployeeExists(int id)
        // {
        //     return _context.Employees.Any(e => e.EmployeeId == id);
        // }
    }
}
