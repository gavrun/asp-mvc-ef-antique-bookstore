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
    [Authorize(Roles = "Sales,Manager")] // work in progress
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager; // Orders by Employee


        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Orders
        // show list of orders 
        public async Task<IActionResult> Index(bool showCancelled = false)
        {
            // get a list of orders with related data included by employee (Manager all, Sales limited by Id)
            // + parameter to optionally show cancelled orders

            // Base query including Sales for calculated property
            var ordersQuery = _context.Orders
                                      .Include(o => o.Customer)
                                      .Include(o => o.Employee)
                                      .Include(o => o.OrderStatus)
                                      .Include(o => o.Sales) // Sales loaded for TotalAmount
                                      .AsQueryable();

            // Role-based filtering
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Sales"))
            {
                var currentEmployee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);

                if (currentEmployee != null)
                {
                    ordersQuery = ordersQuery
                        .Where(o => o.EmployeeId == currentEmployee.Id);
                }
            }

            // Execute the query and project to ViewModel

            if (!showCancelled)
            {
                ordersQuery = ordersQuery
                    .Where(o => o.OrderStatusId != 5); // Exclude Id = 5, Name = "Cancelled"
            }

            var orderViewModels = await ordersQuery
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderIndexViewModel
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    CustomerName = o.Customer != null ? $"{o.Customer.FirstName} {o.Customer.LastName}" : "N/A",
                    EmployeeName = o.Employee != null ? $"{o.Employee.FirstName} {o.Employee.LastName}" : "N/A",
                    StatusName = o.OrderStatus != null ? o.OrderStatus.Name : "N/A",
                    TotalAmount = o.TotalAmount // [NotMapped] TotalAmount property from [Order]
                })
                .ToListAsync();

            // TODO: checkbox filter state to the view cancelled orders
            ViewData["ShowCancelled"] = showCancelled;

            ViewData["OrderStatuses"] = await _context.OrderStatuses
                                                     .Where(os => os.IsActive)
                                                     .OrderBy(os => os.Name)
                                                     .ToListAsync();

            return View(orderViewModels);
        }

        // GET: Orders/Details/5
        // display details of a specific order
        public async Task<IActionResult> Details(int? id)
        {
            // Order by ID with OrderItems and other relations

            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                                      .Include(o => o.Customer)
                                      .Include(o => o.Employee)
                                      .Include(o => o.OrderStatus)
                                      .Include(o => o.PaymentMethod)
                                      .Include(o => o.DeliveryAddress)
                                      .Include(o => o.Sales)
                                      .ThenInclude(s => s.Book)
                                      .Include(o => o.Sales)
                                      .ThenInclude(s => s.SaleEvent)
                                      .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                TempData["ErrorMessage"] = $"Order with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            // Role-based filtering
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Sales"))
            {
                var currentEmployee = await _context.Employees
                                              .FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);

                if (currentEmployee == null || order.EmployeeId != currentEmployee.Id)
                {
                    TempData["ErrorMessage"] = "You are not authorized to view this order.";
                    return RedirectToAction(nameof(Index)); // Forbid();
                }
            }

            // Map to ViewModel
            var viewModel = new OrderDetailsViewModel
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                DeliveryDate = order.DeliveryDate,
                PaymentDate = order.PaymentDate,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer != null ? $"{order.Customer.FirstName} {order.Customer.LastName}" : "N/A",
                EmployeeId = order.EmployeeId,
                EmployeeName = order.Employee != null ? $"{order.Employee.FirstName} {order.Employee.LastName}" : "N/A",
                OrderStatusId = order.OrderStatusId,
                StatusName = order.OrderStatus != null ? order.OrderStatus.Name : "N/A",
                PaymentMethodId = order.PaymentMethodId,
                PaymentMethodName = order.PaymentMethod != null ? order.PaymentMethod.Name : "N/A",
                DeliveryAddressId = order.DeliveryAddressId,
                // Format Delivery Address 
                DeliveryAddressString = order.DeliveryAddress != null
                                ? $"{order.DeliveryAddress.AddressLine1}, {order.DeliveryAddress.City}, {order.DeliveryAddress.PostalCode}"
                                : (order.DeliveryAddressId == null ? "Self-pickup / Not specified" : "Address details missing"), // Handle null FK vs missing related entity
                TotalAmount = order.TotalAmount, // calculated property
                Sales = order.Sales.Select(s => new SaleViewModel
                {
                    BookId = s.BookId,
                    BookTitle = s.Book != null ? s.Book.Title : "Book details missing", // Handle potentially missing Book
                    SalePrice = s.SalePrice,
                    EventName = s.SaleEvent?.Name // Use null-conditional for optional SaleEvent
                }).ToList()
            };

            return View(viewModel);
        }

        // GET: Orders/Create
        // display a form for creating a new order
        public async Task<IActionResult> Create()
        {
            // get data for ViewModel (lists Customer, Employee, OrderStatus, Book, etc.)

            var viewModel = new OrderCreateViewModel();

            await PopulateCreateViewModelSelectLists(viewModel);

            // Role-based filtering
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Sales"))
            {
                if (user != null)
                {
                    var currentEmployee = await _context.Employees
                        .FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);

                    if (currentEmployee != null)
                    {
                        viewModel.SelectedEmployeeId = currentEmployee.Id;
                    }
                }
            }

            // return prepared ViewModel
            return View(viewModel);
        }

        // POST: Orders/Create
        // process form data to create a new order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateViewModel viewModel) // [Bind("...")] or ViewModel or Sales[index].BookId
        {
            // Save an order and its OrderItems

            // Check if at least one sale item is added
            if (viewModel.Sales == null || !viewModel.Sales.Any())
            {
                ModelState.AddModelError("Sales", "Please add at least one book to the order.");
                
                // Re-populate lists before returning view
                await PopulateCreateViewModelSelectLists(viewModel);
                return View(viewModel);
            }

            // Server-side validation
            var requestedBookIds = viewModel.Sales
                .Select(s => s.BookId)
                .Distinct() // distinct just for fun
                .ToList();
            var availableBooks = await _context.Books
                .Where(b => requestedBookIds.Contains(b.Id) && b.StatusId == 1)
                // && b.StatusId == 1 available && b.Quantity >= viewModel.Sales.Count(s => s.BookId == b.Id)
                .ToListAsync();

            // Check if Books found and active
            if (availableBooks.Count != requestedBookIds.Count()) 
            {
                // Find which books are "problematic" (either don't exist or maybe sold if unique)
                var soldBookIds = await _context.Sales
                    .Select(s => s.BookId)
                    .Distinct()
                    .ToListAsync();
                var problematicIds = requestedBookIds
                    .Distinct()
                    .Where(id => !availableBooks.Any(b => b.Id == id) || soldBookIds.Contains(id))
                    .ToList();

                ModelState.AddModelError("Sales", $"One or more selected books (IDs: {string.Join(", ", problematicIds)}) are unavailable or already sold. Please review the items.");

                await PopulateCreateViewModelSelectLists(viewModel);
                return View(viewModel);
            }

            if (ModelState.IsValid)
            {
                // Create Order entity
                var order = new Order
                {
                    OrderDate = viewModel.OrderDate,
                    CustomerId = viewModel.SelectedCustomerId,
                    EmployeeId = viewModel.SelectedEmployeeId,
                    OrderStatusId = viewModel.SelectedOrderStatusId,
                    PaymentMethodId = viewModel.SelectedPaymentMethodId,
                    // DeliveryAddressId = viewModel.SelectedDeliveryAddressId, // To be done
                    
                    Sales = new List<Sale>() // initialize collection here!
                };

                // Create Sale entities and add to Order 
                foreach (var saleItem in viewModel.Sales)
                {
                    var sale = new Sale
                    {
                        BookId = saleItem.BookId,
                        SalePrice = saleItem.SalePrice,
                        // EventId = saleItem.EventId, 
                        
                        Order = order // here
                    };
                    
                    order.Sales.Add(sale);

                    // No Book status/quantity update needed (every Book is unique)
                }

                // Save Order and all related Sales
                try
                {
                    _context.Add(order); 
                    await _context.SaveChangesAsync(); 

                    TempData["SuccessMessage"] = $"Order ID {order.Id} created successfully.";
                    return RedirectToAction(nameof(Details), new { id = order.Id }); // Redirect to the new Order details page
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, see your system administrator.");

                    // Log the error (Serilog or ILogger)
                    // Log.Error(ex, "Error creating order.");
                }
            }

            // Re-populate data for ViewModel when validation fails
            await PopulateCreateViewModelSelectLists(viewModel);

            return View(viewModel);
        }

        // Helper method for populating SelectLists
        private async Task PopulateCreateViewModelSelectLists(OrderCreateViewModel viewModel)
        {
            // Get available entities for dropdowns
            viewModel.Customers = await GetCustomerSelectListAsync(viewModel.SelectedCustomerId);
            viewModel.Employees = await GetEmployeeSelectListAsync(viewModel.SelectedEmployeeId);
            viewModel.OrderStatuses = await GetOrderStatusSelectListAsync(viewModel.SelectedOrderStatusId);
            viewModel.PaymentMethods = await GetPaymentMethodSelectListAsync(viewModel.SelectedPaymentMethodId);

            // Available Books not associated with any Sale
            var soldBookIds = await _context.Sales
                .Select(s => s.BookId)
                .Distinct()
                .ToListAsync();
            
            // Exclude books already added in the current viewModel 
            var currentlyAddedBookIds = viewModel.Sales?
                .Select(s => s.BookId)
                .ToList() ?? new List<int>();
            
            // Combine already sold and currently added
            soldBookIds.AddRange(currentlyAddedBookIds); 

            viewModel.AvailableBooks = new SelectList(
                await _context.Books
                              .Where(b => b.StatusId == 1 && !soldBookIds.Contains(b.Id)) // Filter out sold AND currently added
                              .OrderBy(b => b.Title)
                              .ToListAsync(),
                "Id", "Title");

            // Note: Failed POST attempt
            // The view will need to be able to render the previously entered Sales items if ModelState is invalid.
        }

        // GET: Orders/Edit/5
        // display the order editing form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // receive an order for editing include relations for editing
            var order = await _context.Orders
                .Include(o => o.Sales) 
                .ThenInclude(s => s.Book) // Needs book title for display
                                          // .Include(o => o.Customer)
                                          // .Include(o => o.Employee)
                                          // .Include(o => o.OrderStatus)
                                          // .Include(o => o.PaymentMethod)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                TempData["ErrorMessage"] = $"Order with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            // Role-based filtering
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Sales"))
            {
                var currentEmployee = await _context.Employees
                                              .FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);

                if (currentEmployee == null || order.EmployeeId != currentEmployee.Id)
                {
                    TempData["ErrorMessage"] = "You are not authorized to edit this order.";
                    return RedirectToAction(nameof(Index));
                }
            }

            // get data for ViewModel
            var viewModel = new OrderEditViewModel
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                SelectedCustomerId = order.CustomerId,
                SelectedEmployeeId = order.EmployeeId,
                SelectedOrderStatusId = order.OrderStatusId,
                SelectedPaymentMethodId = order.PaymentMethodId,

                // TODO: Map existing Sales to the ViewModel Sales list
                Sales = order.Sales
                .Select(s => new SaleCreateItemViewModel
                {
                    BookId = s.BookId,
                    SalePrice = s.SalePrice
                    // BookTitle (before JS) 
                    // BookTitle = s.Book?.Title ?? "Unknown Book" 
                    // SelectedDeliveryAddressId = order.DeliveryAddressId
                })
                .ToList()
            };

            // populate SelectLists
            await PopulateEditViewModelSelectLists(viewModel);

            return View(viewModel);
        }

        // Helper method for populating SelectLists for the Edit view
        private async Task PopulateEditViewModelSelectLists(OrderEditViewModel viewModel)
        {
            // Get available entities for dropdowns
            viewModel.Customers = await GetCustomerSelectListAsync(viewModel.SelectedCustomerId);
            viewModel.Employees = await GetEmployeeSelectListAsync(viewModel.SelectedEmployeeId);
            viewModel.OrderStatuses = await GetOrderStatusSelectListAsync(viewModel.SelectedOrderStatusId);
            viewModel.PaymentMethods = await GetPaymentMethodSelectListAsync(viewModel.SelectedPaymentMethodId);

            // Available Books 
            var bookIdsInThisOrder = viewModel.Sales
                .Select(s => s.BookId)
                .ToList();

            viewModel.AvailableBooks = new SelectList(
                await _context.Books
                              .Where(b => b.StatusId == 1 && !bookIdsInThisOrder.Contains(b.Id)) // Available and NOT in this Order
                              .OrderBy(b => b.Title)
                              .ToListAsync(),
                "Id", "Title");

            // TODO: DeliveryAddresses need specific logic based on SelectedCustomerId 
        }

        // POST: Orders/Edit/5
        // process form data for order editing
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderEditViewModel viewModel) // [Bind("...")] or ViewModel or Sales[index].BookId
        {

            // Ensure the ID from the route matches the ID in the ViewModel
            if (id != viewModel.Id)
            {
                return BadRequest(); 
            }

            // Load original Order including current Sales for comparing/updating
            var orderToUpdate = await _context.Orders
                    .Include(o => o.Sales) 
                    .FirstOrDefaultAsync(o => o.Id == id);

            if (orderToUpdate == null)
            {
                TempData["ErrorMessage"] = $"Order with ID {id} not found (possibly deleted).";
                return NotFound($"Order with ID {id} not found.");
            }

            // 3. Authorization Check (Sales can only edit their own orders)
            //    Re-check authorization against the loaded entity
            // Role-based filtering
            if (User.IsInRole("Sales"))
            {
                var user = await _userManager.GetUserAsync(User);
                var currentEmployee = await _context.Employees
                                              .FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);

                if (currentEmployee == null || orderToUpdate.EmployeeId != currentEmployee.Id)
                {
                    return RedirectToAction(nameof(Details), new { id = id }); 
                }
                
                viewModel.SelectedEmployeeId = orderToUpdate.EmployeeId;
            }

            // Validations for Book
            if (viewModel.Sales == null || !viewModel.Sales.Any())
            {
                ModelState.AddModelError("Sales", "The order must contain at least one book.");
            }
            else 
            {
                var requestedBookIds = viewModel.Sales
                    .Select(s => s.BookId)
                    .Distinct()
                    .ToList();
                // This is a business model strict check.
                var availableBooksFound = await _context.Books
                                                 .Where(b => requestedBookIds.Contains(b.Id) && b.StatusId == 1)
                                                 .Select(b => b.Id)
                                                 .ToListAsync();

                if (availableBooksFound.Count != requestedBookIds.Count)
                {
                    var problematicIds = requestedBookIds.Except(availableBooksFound)
                        .ToList();

                    ModelState.AddModelError("Sales", $"One or more selected books (IDs: {string.Join(", ", problematicIds)}) are no longer available (Status is not 'Available' or book does not exist). Please review the items.");
                }
            }

            // Edit
            if (ModelState.IsValid)
            {
                // Update scalar properties of the Order by manual mapping which gives more control
                orderToUpdate.OrderDate = viewModel.OrderDate;
                orderToUpdate.CustomerId = viewModel.SelectedCustomerId;
                orderToUpdate.EmployeeId = viewModel.SelectedEmployeeId; 
                orderToUpdate.OrderStatusId = viewModel.SelectedOrderStatusId;
                orderToUpdate.PaymentMethodId = viewModel.SelectedPaymentMethodId;
                // orderToUpdate.DeliveryAddressId = viewModel.SelectedDeliveryAddressId;

                // Update related Sales 
                await UpdateOrderSales(orderToUpdate, viewModel.Sales);

                // Save 
                try
                {
                    await UpdateBookStatusesForOrder(orderToUpdate.Sales, orderToUpdate.Id);

                    await _context.SaveChangesAsync(); 

                    TempData["SuccessMessage"] = $"Order ID {orderToUpdate.Id} updated successfully.";
                    return RedirectToAction(nameof(Details), new { id = orderToUpdate.Id });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Log conflicts
                    // _logger.LogWarning(ex, "Concurrency conflict occurred while updating Order ID {OrderId}.", orderToUpdate.Id);

                    var entry = ex.Entries.Single();
                    var databaseValues = await entry.GetDatabaseValuesAsync();

                    if (databaseValues == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The order was deleted by another user.");
                    }
                    else
                    {
                        // On conflict, load values into a temporary object
                        var databaseOrder = (Order)databaseValues.ToObject();

                        // Add specific errors for conflicting fields 
                        if (databaseOrder.OrderStatusId != orderToUpdate.OrderStatusId) // Check original loaded value
                            ModelState.AddModelError("SelectedOrderStatusId", $"Current value in database: {(await _context.OrderStatuses.FindAsync(databaseOrder.OrderStatusId))?.Name ?? "N/A"}");

                        ModelState.AddModelError(string.Empty, "The order you attempted to edit "
                            + "was modified by another user after you got the original value. "
                            + "Your edit operation was canceled and the current values in the database have been displayed. "
                            + "If you still want to edit this record, click the Save button again. Otherwise click Cancel.");

                        // Refresh ViewModel with values to show the conflict
                        viewModel.OrderDate = databaseOrder.OrderDate;
                        viewModel.SelectedCustomerId = databaseOrder.CustomerId;
                        viewModel.SelectedEmployeeId = databaseOrder.EmployeeId;
                        viewModel.SelectedOrderStatusId = databaseOrder.OrderStatusId;
                        viewModel.SelectedPaymentMethodId = databaseOrder.PaymentMethodId;
                        // Set concurrency token for next attempt if using row versioning
                        // orderToUpdate.RowVersion = databaseOrder.RowVersion;
                        // Remove the original ID value from ModelState if using RowVersion
                        ModelState.Remove("Id"); 
                    }
                }
                catch (DbUpdateException ex)
                {
                    // Log the detailed error
                    // _logger.LogError(ex, "Error updating Order ID {OrderId}.", orderToUpdate.Id);

                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, see your system administrator. " +
                        $"Error: {ex.InnerException?.Message ?? ex.Message}"); 
                }
            } 

            // repopulate lists 
            await PopulateEditViewModelSelectLists(viewModel);

            return View(viewModel);
        }

        // Helper method to synchronize the Sales collection
        private async Task UpdateOrderSales(Order orderToUpdate, List<SaleCreateItemViewModel> viewModelSales)
        {
            if (viewModelSales == null) // defensive check
            {
                viewModelSales = new List<SaleCreateItemViewModel>();
            }

            var viewModelBookIds = viewModelSales
                .Select(s => s.BookId)
                .ToList();
            var existingBookIds = orderToUpdate.Sales
                .Select(s => s.BookId)
                .ToList();

            // Find sales that are NOT in the submitted ViewModel
            var salesToRemove = orderToUpdate.Sales
                .Where(s => !viewModelBookIds.Contains(s.BookId))
                .ToList(); // Materialize list before removing

            foreach (var saleToRemove in salesToRemove)
            {
                orderToUpdate.Sales
                    .Remove(saleToRemove); // Remove from collection
                
                _context.Sales
                    .Remove(saleToRemove); // Mark for deletion in context
               
                // Book status update (back to Available) handled in UpdateBookStatusesForOrder()
            }

            // Update existing Sales and add new Sales
            foreach (var viewModelSale in viewModelSales)
            {
                var existingSale = orderToUpdate.Sales
                    .FirstOrDefault(s => s.BookId == viewModelSale.BookId);

                if (existingSale != null)
                {
                    // Update existing Sale
                    if (existingSale.SalePrice != viewModelSale.SalePrice)
                    {
                        existingSale.SalePrice = viewModelSale.SalePrice;
                        _context.Entry(existingSale).State = EntityState.Modified;
                    }
                }
                else
                {
                    // Add new Sale
                    var newSale = new Sale
                    {
                        OrderId = orderToUpdate.Id, // Explicitly set FK
                        BookId = viewModelSale.BookId,
                        SalePrice = viewModelSale.SalePrice,
                        // EventId = viewModelSale.EventId 
                        // Order navigation property (orderToUpdate) will be set by EF Core
                    };
                    orderToUpdate.Sales.Add(newSale); // Add to the Order's collection
                    // _context.Sales.Add(newSale); // Not strictly necessary if added to tracked collection
                    // Book status update (to Sold/Reserved) handled in UpdateBookStatusesForOrder
                }
            }
        }

        // Helper method to update Book statuses
        private async Task UpdateBookStatusesForOrder(ICollection<Sale> finalSales, int orderId)
        {
            // Business rules ([x] unique books vs [ ] quantity)

            // Get all Book IDs currently associated with this order AFTER updates
            var bookIdsInOrder = finalSales
                .Select(s => s.BookId)
                .Distinct()
                .ToList();

            // Get Book IDs that were previously in this order but are now removed

            // Find all books involved (currently in order + potentially removed ones if status needs revertting)

            var booksToUpdate = await _context.Books
                .Where(b => bookIdsInOrder
                .Contains(b.Id))
                .ToListAsync();

            foreach (var book in booksToUpdate)
            {
                // Set to 'Sold' (or 'Reserved')
                if (book.StatusId == 1) // Only update if it was 'Available'
                {
                    book.StatusId = 3; 
                    _context.Entry(book).State = EntityState.Modified;
                }
            }

            // TODO: Add logic here to find books that were REMOVED from the order
        }

        // Helper methods for SelectList
        private async Task<SelectList> GetCustomerSelectListAsync(int? selectedId)
        {
            return new SelectList(
                await _context.Customers
                              .Where(c => c.IsActive)
                              .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
                              .Select(c => new { c.Id, Name = $"{c.LastName}, {c.FirstName}" })
                              .AsNoTracking()
                              .ToListAsync(),
                "Id", "Name", selectedId);
        }
        private async Task<SelectList> GetEmployeeSelectListAsync(int? selectedId)
        {
            return new SelectList(
                await _context.Employees
                              .Where(e => e.IsActive)
                              .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
                              .Select(e => new { e.Id, Name = $"{e.LastName}, {e.FirstName}" })
                              .AsNoTracking()
                              .ToListAsync(),
                "Id", "Name", selectedId);
        }
        private async Task<SelectList> GetOrderStatusSelectListAsync(int? selectedId)
        {
            return new SelectList(
                await _context.OrderStatuses
                              .Where(os => os.IsActive)
                              .OrderBy(os => os.Name)
                              .AsNoTracking()
                              .ToListAsync(),
                "Id", "Name", selectedId);
        }
        private async Task<SelectList> GetPaymentMethodSelectListAsync(int? selectedId)
        {
            return new SelectList(
               await _context.PaymentMethods
                             .Where(pm => pm.IsActive)
                             .OrderBy(pm => pm.Name)
                             .AsNoTracking()
                             .ToListAsync(),
               "Id", "Name", selectedId);
        }

        // GET: Orders/Cancel/5
        // Display confirmation page for cancelling an order
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Load order details needed for confirmation display
            var orderToCancel = await _context.Orders
                                        .Include(o => o.Customer)
                                        .Include(o => o.Employee)
                                        .Include(o => o.OrderStatus) // Include status to check if cancelled
                                        .FirstOrDefaultAsync(m => m.Id == id);

            if (orderToCancel == null)
            {
                TempData["ErrorMessage"] = $"Order with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            // Role-based Authorization check (Manager all, Sales limited by Id)
            bool isAuthorized = false;
            if (User.IsInRole("Manager"))
            {
                isAuthorized = true;
            }
            else if (User.IsInRole("Sales"))
            {
                var user = await _userManager.GetUserAsync(User);
                var currentEmployee = await _context.Employees
                                              .FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);
                if (currentEmployee != null && orderToCancel.EmployeeId == currentEmployee.Id)
                {
                    isAuthorized = true;
                }
            }
            if (!isAuthorized)
            {
                TempData["ErrorMessage"] = "You are not authorized to cancel this order.";
                return RedirectToAction(nameof(Index));
            }
            // End Role-based Authorization check

            // Check if already cancelled
            if (orderToCancel.OrderStatusId == 5) 
            {
                TempData["WarningMessage"] = $"Order ID {id} is already cancelled.";
                return RedirectToAction(nameof(Details), new { id = id }); // Redirect to details ?
            }

            return View(orderToCancel); 
        }

        // POST: Orders/Cancel/5
        // Confirms and processes the order cancellation
        [HttpPost, ActionName("Cancel")] // to map Cancel.cshtml form 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            // Load Order with Sales and related Books to update Book statuses
            var orderToCancel = await _context.Orders
                .Include(o => o.Sales)
                .ThenInclude(s => s.Book)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orderToCancel == null)
            {
                // Order already cancelled or deleted 
                TempData["ErrorMessage"] = $"Order with ID {id} not found (possibly deleted).";
                return RedirectToAction(nameof(Index));
            }

            // Role-based Authorization check
            bool isAuthorized = false;
            if (User.IsInRole("Manager"))
            {
                isAuthorized = true;
            }
            else if (User.IsInRole("Sales"))
            {
                var user = await _userManager.GetUserAsync(User);
                var currentEmployee = await _context.Employees
                                              .FirstOrDefaultAsync(e => e.ApplicationUserId == user.Id);
                if (currentEmployee != null && orderToCancel.EmployeeId == currentEmployee.Id)
                {
                    isAuthorized = true;
                }
            }
            if (!isAuthorized)
            {
                TempData["ErrorMessage"] = "You are not authorized to cancel this order.";
                return RedirectToAction(nameof(Index));
            }
            // End Role-based Authorization check

            // Double-check if already cancelled (race condition)
            if (orderToCancel.OrderStatusId == 5)
            {
                TempData["WarningMessage"] = $"Order ID {id} was already cancelled.";
                return RedirectToAction(nameof(Index));
            }

            // Cancel
            try
            {
                // Change status
                orderToCancel.OrderStatusId = 5; // Set to "Cancelled" status 
                _context.Entry(orderToCancel).State = EntityState.Modified;

                // Update Book
                foreach (var sale in orderToCancel.Sales)
                {
                    if (sale.Book != null)
                    {
                        // Only set back to Available if it's currently linked to this Order
                        // (e.g., status might be Sold=3 or Reserved=2 to another order)
                        if (sale.Book.StatusId != 1) // If not already Available
                        {
                            sale.Book.StatusId = 1;
                            _context.Entry(sale.Book).State = EntityState.Modified;
                        }
                    }
                }

                // Save 
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Order ID {orderToCancel.Id} has been cancelled successfully.";
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle concurrency conflict 
                // _logger.LogWarning(ex, "Concurrency conflict cancelling Order ID {OrderId}", orderToCancel.Id);
                TempData["ErrorMessage"] = "Could not cancel the order. It was modified or deleted by another user. Please try again.";
                
                return RedirectToAction(nameof(Details), new { id = id });
            }
            catch (DbUpdateException ex)
            {
                // _logger.LogError(ex, "Error cancelling Order ID {OrderId}", orderToCancel.Id);
                TempData["ErrorMessage"] = "An error occurred while cancelling the order. Please try again or contact support.";

                return RedirectToAction(nameof(Details), new { id = id });
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Orders/Delete/5
        // display confirmation of order deletion
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // order details to confirm deletion
            // OrderExists(id);

            // INFO: not implemented

            return View(new Order { Id = id.Value }); // stub
        }

        // POST: Orders/Delete/5
        // delete order 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // delete an order (and possibly linked OrderItems)

            // INFO: not implemented

            return RedirectToAction(nameof(Index));
        }

        // Helper method 
        private bool OrderExists(int id)
        {
            // check the existence of an Order

            // INFO: not implemented

            return false; // stub
        }

    }
}
