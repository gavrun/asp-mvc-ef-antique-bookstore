using AntiqueBookstore.Data;
using AntiqueBookstore.Domain.Entities;
using AntiqueBookstore.Models;
using AntiqueBookstore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AntiqueBookstore.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly ILogger<BooksController> _logger; // Logger for file upload
        private readonly IFileStorageService _fileStorageService; 

        public BooksController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, 
            ILogger<BooksController> logger, IFileStorageService fileStorageService)
        {
            // safer way to inject dependencies

            _context = context ?? throw new ArgumentNullException(nameof(context));

            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));

            _logger = logger;

            _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
        }

        // GET: /Books or /Books/Index
        public async Task<IActionResult> Index()
        {
            // TODO: fetch books from the database
            var books = await _context.Books
                .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
                .Include(b => b.Condition)
                .Include(b => b.Status)
                .OrderBy(b => b.Title)
                .ToListAsync();

            return View(books);

            //return View();
        }

        // GET: Books/Create
        public async Task<IActionResult> Create()
        {
            // TODO: refactor Repopulate 
            var viewModel = new BookCreateViewModel();

            await PopulateViewModelListsAsync(viewModel);

            // Other lists (genres, states), query and add here

            return View(viewModel);
        }

        private async Task PopulateViewModelListsAsync(BookCreateViewModel model)
        {
            // get data from Db
            var authors = await _context.Authors
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .Select(a => new { Id = a.Id, DisplayName = a.FirstName + " " + a.LastName }) // Projection to an anonymous type {"Key", "Data"}
                .ToListAsync();

            var conditions = await _context.BookConditions
                .OrderBy(c => c.Name)
                .ToListAsync();

            var statuses = await _context.BookStatuses
                .OrderBy(s => s.Name)
                .ToListAsync();

            // create SelectList and MultiSelectList to appoint to ViewModel
            model.Authors = new MultiSelectList(authors, "Id", "DisplayName", model.SelectedAuthorIds);
            model.Conditions = new SelectList(conditions, "Id", "Name", model.ConditionId);
            model.Statuses = new SelectList(statuses, "Id", "Name", model.StatusId);
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookCreateViewModel viewModel)
        {
            // Server-side validation
            if (!ModelState.IsValid)
            {
                // NOTE: Repopulate
                // If the model state is NOT valid (e.g., required field missing, range error):
                // It's CRUCIAL to repopulate the dropdown lists before returning the view,
                // otherwise, they will be empty when the form is redisplayed with errors.

                await PopulateViewModelListsAsync(viewModel);

                // Same form with the input preserved and validation error messages
                return View(viewModel); 
            }

            // TODO: Refactor File Upload logic to a Services layer

            // File Upload
            // Variable to store the relative path of the saved image for the database
            string? relativeImagePath = null;

            if (viewModel.CoverImageFile != null && viewModel.CoverImageFile.Length > 0)
            {
                _logger.LogInformation("Processing uploaded file: {FileName}", viewModel.CoverImageFile.FileName);

                var fileUploadResult = await _fileStorageService.SaveFileAsync(viewModel.CoverImageFile, "images/covers");

                if (!fileUploadResult.Success)
                {
                    ModelState.AddModelError(nameof(viewModel.CoverImageFile), fileUploadResult.ErrorMessage ?? "File upload failed.");
                    _logger.LogWarning("File upload failed. Error: {Error}", fileUploadResult.ErrorMessage);
                }
                else
                {
                    relativeImagePath = fileUploadResult.RelativePath;
                    _logger.LogInformation("File uploaded successfully. Path: {Path}", relativeImagePath);
                }
            }
            else
            {
                // No file uploaded ModelState
                _logger.LogInformation("No file was uploaded.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateViewModelListsAsync(viewModel);

                return View(viewModel);
            }

            // TODO: AutoMapper can be used to map BookViewModel to Book entity as another Mapping layer

            // Mapping ViewModel to Book
            var book = new Book
            {
                Title = viewModel.Title,
                Publisher = viewModel.Publisher,             
                PublicationDate = viewModel.PublicationDate, 
                PurchasePrice = viewModel.PurchasePrice,     
                RecommendedPrice = viewModel.RecommendedPrice,
                ConditionId = viewModel.ConditionId,       
                StatusId = viewModel.StatusId, 
                
                CoverImagePath = relativeImagePath // can be null
            };

            // TODO: Database related logic (Add Book, Add BookAuthors, SaveChanges) can be on a dedicated Service layer.

            // Saving Db
            try
            {
                _context.Add(book);

                await _context.SaveChangesAsync();

                if (viewModel.SelectedAuthorIds != null && viewModel.SelectedAuthorIds.Any())
                {
                    foreach (var authorId in viewModel.SelectedAuthorIds)
                    {
                        // joint entity, tuple
                        var bookAuthor = new BookAuthor
                        {
                            BookId = book.Id, 
                            AuthorId = authorId
                        };

                        _context.BookAuthors.Add(bookAuthor); 
                    }
                    // Save BookAuthor records
                    await _context.SaveChangesAsync();
                }

                // Success message by TempData
                TempData["SuccessMessage"] = $"Book '{book.Title}' created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                await PopulateViewModelListsAsync(viewModel);

                return View(viewModel);
            }
            //return (IActionResult)Task.CompletedTask;
        }

    }
}
