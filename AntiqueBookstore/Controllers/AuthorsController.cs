using AntiqueBookstore.Data;
using AntiqueBookstore.Domain.Entities;
using AntiqueBookstore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AntiqueBookstore.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger<AuthorsController> _logger; // Logger for AJAX


        public AuthorsController(ApplicationDbContext context, ILogger<AuthorsController> logger)
        {
            _context = context;
            _logger = logger; 
        }

        // GET: Authors
        public async Task<IActionResult> Index()
        {
            // Retrieves all authors from the database 
            var authors = await _context.Authors.ToListAsync();

            return View(authors);
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound(); // 404 page, if not found
            }

            // Finds the author by ID
            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.Id == id);

            if (author == null)
            {
                return NotFound(); 
            }

            return View(author);
        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            // Просто отображаем пустое представление с формой
            return View();
        }

        // POST: Authors/Create
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF protection
        public async Task<IActionResult> Create([Bind("FirstName,LastName,BirthYear,DeathYear,Bio")] Author author) // All Author's properties to bind
        {
            // Validation check specified in the Author class or attributes (on server)
            if (ModelState.IsValid)
            {
                // add to context
                _context.Add(author);

                // save changes to the database
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }

            // If the model is invalid, return the user to the same form to correct the errors
            // The data entered will be kept in the form
            return View(author);
        }

        // GET: /Authors/_CreateAuthorPartial
        [HttpGet]
        public IActionResult GetAuthorCreatePartial()
        {
            // empty model
            return PartialView("_CreateAuthorPartial", new AuthorCreateAjaxViewModel());
        }

        // POST: /Authors/CreateAuthorAjax
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAuthorAjax([Bind("FirstName,LastName,BirthYear,DeathYear,Bio")] AuthorCreateAjaxViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var author = new Author
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    BirthYear = viewModel.BirthYear,
                    DeathYear = viewModel.DeathYear,
                    Bio = viewModel.Bio
                };

                try
                {
                    _context.Authors.Add(author);

                    await _context.SaveChangesAsync();

                    // OKAY return JSON with ID and display "Full Name"
                    return Json(new
                    {
                        success = true,
                        id = author.Id,
                        displayName = $"{author.FirstName} {author.LastName}"
                    });
                }
                catch (DbUpdateException ex)
                {
                    // Ошибка БД
                    _logger.LogError(ex, "Error saving new Author via AJAX.");
                    return Json(new { success = false, message = "Database error occurred while saving the author." });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error saving new author via AJAX.");
                    return Json(new { success = false, message = "An unexpected error occurred." });
                }
            }
            // INVALIDATED return JSON error or PartialView errors
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .ToList();
            // JsonResult
            return Json(new { success = false, errors = errors });
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find the author by ID
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound();
            }
            
            return View(author);
        }

        // POST: Authors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,BirthYear,DeathYear,Bio")] Author author) // Needs Id in Bind()
        {
            // Check if the ID from the route matches the ID in the model
            if (id != author.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tell EF Core that this entity needs to be updated
                    _context.Update(author);

                    // Save changes to the database
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handle the case if the entry was modified or deleted by another user while the current user was editing it
                    if (!AuthorExists(author.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // Explicit error
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                            + "was modified by another user after you got the original value. "
                            + "The edit operation was canceled. If you still want to edit this record, please go back and try again.");
                        
                        // Return to edit
                        return View(author);
                        //throw;
                    }
                }
                
                return RedirectToAction(nameof(Index));
            }
            // Return to edit
            return View(author);
        }

        // Helper method to check if the author exists
        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find the author to confirm the deletion
            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            // Pass the author to the confirmation view
            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")] // This method handles POST request for the Delete action.
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find the author that needs to be removed
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);

                await _context.SaveChangesAsync();
            }
            else
            {
                // If already gone
                return NotFound();
            }

            // Перенаправляем на список авторов
            return RedirectToAction(nameof(Index));
        }
    }
}
