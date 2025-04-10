using AntiqueBookstore.Data;
using AntiqueBookstore.Domain.Entities;
using AntiqueBookstore.Models;
using AntiqueBookstore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;

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

        // created helper with BookCreateViewModel 
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

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details action called with null id.");

                // BadRequest()
                return NotFound();
            }

            // Loading the book with its related data
            var book = await _context.Books
                .Include(b => b.Condition)
                .Include(b => b.Status)
                .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                _logger.LogWarning("Book with id {BookId} not found for Details action.", id);
                return NotFound();
            }

            // no BookDetailsViewModel exists for Details
            return View(book);
        }

        // overloaded helper BookEditViewModel
        private async Task PopulateViewModelListsAsync(BookEditViewModel viewModel)
        {
            // same here
            var authors = await _context.Authors
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .Select(a => new { Id = a.Id, DisplayName = a.FirstName + " " + a.LastName })
                .ToListAsync();

            var conditions = await _context.BookConditions
                .OrderBy(c => c.Name)
                .ToListAsync();

            var statuses = await _context.BookStatuses
                .OrderBy(s => s.Name)
                .ToListAsync();

            // for BookEditViewModel properties
            viewModel.AuthorsList = new MultiSelectList(authors, "Id", "DisplayName", viewModel.SelectedAuthorIds);
            viewModel.ConditionsList = new SelectList(conditions, "Id", "Name", viewModel.ConditionId);
            viewModel.StatusesList = new SelectList(statuses, "Id", "Name", viewModel.StatusId);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit GET action called with null id.");
                return NotFound();
            }

            // Находим книгу, включая ее текущих авторов
            var book = await _context.Books
                                     .Include(b => b.BookAuthors)
                                     .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                _logger.LogWarning("Book with id {BookId} not found for Edit GET action.", id);
                return NotFound();
            }

            var viewModel = new BookEditViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Publisher = book.Publisher,
                PublicationDate = book.PublicationDate,
                PurchasePrice = book.PurchasePrice,
                RecommendedPrice = book.RecommendedPrice,
                ConditionId = book.ConditionId,
                StatusId = book.StatusId,
                ExistingCoverImagePath = book.CoverImagePath,
                // authors list
                SelectedAuthorIds = book.BookAuthors.Select(ba => ba.AuthorId).ToList()
            };

            // Fill dropdowns/multi-select
            await PopulateViewModelListsAsync(viewModel);

            return View(viewModel);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Title,Publisher,PublicationDate,PurchasePrice,RecommendedPrice,ConditionId,StatusId,SelectedAuthorIds,ExistingCoverImagePath")] BookEditViewModel viewModel,
            IFormFile? coverImageFile) // matches <input type="file">
        {
            if (id != viewModel.Id)
            {
                _logger.LogWarning("ID mismatch in Edit POST action. Route id: {RouteId}, ViewModel id: {ViewModelId}", id, viewModel.Id);
                return NotFound();
            }

            var bookToUpdate = await _context.Books
                                             .Include(b => b.BookAuthors)
                                             .FirstOrDefaultAsync(b => b.Id == id);

            if (bookToUpdate == null)
            {
                _logger.LogWarning("Book with id {BookId} not found for Edit POST action.", id);
                return NotFound();
            }

            // remove validation errors related to collections that we fill programmatically
            ModelState.Remove("ConditionsList");
            ModelState.Remove("StatusesList");
            ModelState.Remove("AuthorsList");

            if (ModelState.IsValid)
            {
                try
                {
                    if (coverImageFile != null && coverImageFile.Length > 0)
                    {
                        _logger.LogInformation("New cover image uploaded for book id {BookId}. Processing...", id);

                        // new image uploaded for cover image 
                        if (!string.IsNullOrEmpty(bookToUpdate.CoverImagePath))
                        {
                            _logger.LogInformation("Attempting to delete old cover image '{ImagePath}' for book id {BookId}.", bookToUpdate.CoverImagePath, id);
                            FileDeleteResult deleteResult = await _fileStorageService.DeleteFileAsync(bookToUpdate.CoverImagePath);

                            if (!deleteResult.Success)
                            {
                                _logger.LogError("Error deleting old cover image '{ImagePath}' for book id {BookId}: {ErrorMessage}",
                                                 bookToUpdate.CoverImagePath, id, deleteResult.ErrorMessage);
                            }
                            else
                            {
                                _logger.LogInformation("Old cover image '{ImagePath}' deleted (or did not exist) for book id {BookId}.", bookToUpdate.CoverImagePath, id);
                            }
                        }

                        // save the new file
                        _logger.LogInformation("Saving new cover image for book id {BookId}...", id);
                        FileUploadResult saveResult = await _fileStorageService.SaveFileAsync(coverImageFile, "images/covers");

                        if (!saveResult.Success)
                        {
                            _logger.LogError("Failed to save new cover image for book id {BookId}.", id);
                            ModelState.AddModelError("CoverImageFile", "Error uploading cover image.");

                            await PopulateViewModelListsAsync(viewModel);
                            return View(viewModel);
                        }

                        // update cover image path
                        bookToUpdate.CoverImagePath = saveResult.RelativePath;
                        _logger.LogInformation("New cover image '{ImagePath}' saved and path updated for book id {BookId}.", bookToUpdate.CoverImagePath, id);

                    }
                    else
                    {
                        // do nothing
                        _logger.LogInformation("No new cover image uploaded for book id {BookId}. Existing image path remains.", id);
                    }

                    // update Book by BookEditViewModel
                    bookToUpdate.Title = viewModel.Title;
                    bookToUpdate.Publisher = viewModel.Publisher;
                    bookToUpdate.PublicationDate = viewModel.PublicationDate;
                    bookToUpdate.PurchasePrice = viewModel.PurchasePrice;
                    bookToUpdate.RecommendedPrice = viewModel.RecommendedPrice;
                    bookToUpdate.ConditionId = viewModel.ConditionId;
                    bookToUpdate.StatusId = viewModel.StatusId;
                    // update Book many-to-many
                    await UpdateBookAuthors(bookToUpdate, viewModel.SelectedAuthorIds);
                    
                    // save 
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Book with id {BookId} updated successfully.", id);
                    TempData["SuccessMessage"] = "Book updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency error updating book with id {BookId}.", id);

                    // potential conflicts
                    var entry = ex.Entries.Single();
                    var databaseValues = await entry.GetDatabaseValuesAsync();

                    if (databaseValues == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The book was deleted by another user.");
                    }
                    else
                    {
                        // var databaseEntity = (Book)databaseValues.ToObject();
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                           + "was modified by another user after you got the original value. "
                           + "Your edit operation was canceled. If you still want to edit this record, "
                           + "click the 'Cancel' button to reload the page.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating book with id {BookId}.", id);
                    ModelState.AddModelError("", "An unexpected error occurred while saving the book. Please try again.");
                }
            }
            else
            {
                _logger.LogWarning("ModelState is invalid for Edit POST action for book id {BookId}.", id);

                foreach (var modelStateKey in ModelState.Keys)
                {
                    var value = ModelState[modelStateKey];
                    foreach (var error in value.Errors)
                    {
                        _logger.LogWarning("Validation Error for {Key}: {ErrorMessage}", modelStateKey, error.ErrorMessage);
                    }
                }
            }

            await PopulateViewModelListsAsync(viewModel);
            return View(viewModel);
        }

        // helper Book-Author (Many-to-Many)
        private async Task UpdateBookAuthors(Book bookToUpdate, List<int> selectedAuthorIds)
        {
            if (selectedAuthorIds == null)
            {
                // TODO: delete all mappings if no Authors selected
                bookToUpdate.BookAuthors.Clear(); 
                _logger.LogInformation("All authors removed for book id {BookId}.", bookToUpdate.Id);
                return;
            }

            var currentAuthorIds = bookToUpdate.BookAuthors.Select(ba => ba.AuthorId).ToHashSet();
            var selectedAuthorIdsSet = selectedAuthorIds.ToHashSet();

            // Authors to remove (present in current, not selected)
            var authorsToRemove = bookToUpdate.BookAuthors
                                              .Where(ba => !selectedAuthorIdsSet.Contains(ba.AuthorId))
                                              .ToList();

            foreach (var authorToRemove in authorsToRemove)
            {
                // Delete the linked record
                _context.BookAuthors.Remove(authorToRemove); 
                _logger.LogDebug("Removing author id {AuthorId} from book id {BookId}.", authorToRemove.AuthorId, bookToUpdate.Id);
            }

            // Authors to add (selected, not in current)
            var authorIdsToAdd = selectedAuthorIdsSet.Where(id => !currentAuthorIds.Contains(id)).ToList();
            foreach (var authorIdToAdd in authorIdsToAdd)
            {
                // Protect, check if an author with this ID exists 
                var authorExists = await _context.Authors.AnyAsync(a => a.Id == authorIdToAdd);
                if (authorExists)
                {
                    bookToUpdate.BookAuthors.Add(new BookAuthor { BookId = bookToUpdate.Id, AuthorId = authorIdToAdd });
                    _logger.LogDebug("Adding author id {AuthorId} to book id {BookId}.", authorIdToAdd, bookToUpdate.Id);
                }
                else
                {
                    _logger.LogWarning("Attempted to add non-existent author id {AuthorId} to book id {BookId}.", authorIdToAdd, bookToUpdate.Id);
                }
            }
            _logger.LogInformation("BookAuthors updated for book id {BookId}. Added: {AddedCount}, Removed: {RemovedCount}",
                                   bookToUpdate.Id, authorIdsToAdd.Count, authorsToRemove.Count);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete GET action called with null id.");
                return NotFound();
            }

            // Upload Book data sufficient for identification upon deletion confirmation
            var book = await _context.Books
                .Include(b => b.Condition) 
                .Include(b => b.Status)  
                .Include(b => b.BookAuthors)  
                    .ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                _logger.LogWarning("Book with id {BookId} not found for Delete GET action.", id);
                return NotFound();
            }

            return View(book); 
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")] // ActionName("Delete") matters if the method is called differently (DeleteConfirmed) asp-action="Delete"
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find Book by id, including linked data
            var bookToDelete = await _context.Books
                                     .Include(b => b.Sales) // check if the Book is referenced in Sales
                                     .FirstOrDefaultAsync(b => b.Id == id);

            if (bookToDelete == null)
            {
                _logger.LogWarning("Book with id {BookId} not found for DeleteConfirmed action.", id);
                // TempData message if Book not found
                TempData["ErrorMessage"] = "Book not found. It might have been deleted already.";
                return RedirectToAction(nameof(Index));
            }

            // Checking according to DeleteBehavior.Restrict
            if (bookToDelete.Sales.Any())
            {
                _logger.LogWarning("Attempted to delete book id {BookId} which has associated sales records.", id);
                TempData["ErrorMessage"] = $"Cannot delete book '{bookToDelete.Title}' because it has associated sales records. Please remove sales records first.";

                return RedirectToAction(nameof(Index));
            }

            try
            {
                // remove cover image
                string? coverImagePath = bookToDelete.CoverImagePath; 
                if (!string.IsNullOrEmpty(coverImagePath))
                {
                    _logger.LogInformation("Attempting to delete cover image '{ImagePath}' for book id {BookId}.", coverImagePath, id);
                    FileDeleteResult deleteResult = await _fileStorageService.DeleteFileAsync(coverImagePath);
                    if (!deleteResult.Success)
                    {
                        _logger.LogError("Error deleting cover image '{ImagePath}' for book id {BookId}: {ErrorMessage}",
                                         coverImagePath, id, deleteResult.ErrorMessage);
                        TempData["WarningMessage"] = $"Book deleted, but failed to delete cover image: {deleteResult.ErrorMessage}";
                    }
                    else
                    {
                        _logger.LogInformation("Cover image '{ImagePath}' deleted (or did not exist) for book id {BookId}.", coverImagePath, id);
                    }
                }

                // remove book with Cascade Delete for BookAuthors
                _context.Books.Remove(bookToDelete);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Book with id {BookId} deleted successfully.", id);
                TempData["SuccessMessage"] = $"Book '{bookToDelete.Title}' deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex) 
            {
                _logger.LogError(ex, "Error deleting book with id {BookId}.", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the book. It might be related to other data linked to it.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) // Обработка других неожиданных ошибок
            {
                _logger.LogError(ex, "Unexpected error deleting book with id {BookId}.", id);
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the book.";
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
