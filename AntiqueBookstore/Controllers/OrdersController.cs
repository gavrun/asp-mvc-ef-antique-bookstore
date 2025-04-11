using AntiqueBookstore.Data;
using AntiqueBookstore.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AntiqueBookstore.Controllers
{
     [Authorize(Roles = "Sales,Manager")] // work in progress
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;


        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Orders
        // show list of orders
        public async Task<IActionResult> Index()
        {
            // TODO: get a list of orders with related data included

            // var orders = await _context.Orders.Include(o => o.Customer).Include(o => o.Employee).Include(o => o.OrderStatus).ToListAsync();
            // return View(orders);

            return View(new List<Order>()); // stub
        }

        // GET: Orders/Details/5
        // display details of a specific order
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // TODO: receive an order by ID with OrderItems and other relationships

            // var order = await _context.Orders
            //     .Include(o => o.Customer)
            //     .Include(o => o.Employee)
            //     .Include(o => o.OrderStatus)
            //     .Include(o => o.OrderItems)
            //         .ThenInclude(oi => oi.Book)
            //     .FirstOrDefaultAsync(m => m.OrderId == id);
            // if (order == null)
            // {
            //     return NotFound();
            // }
            // return View(order);

            return View(new Order { Id = id.Value }); // stub
        }

        // GET: Orders/Create
        // display a form for creating a new order
        public IActionResult Create()
        {
            // TODO: get data for ViewModel (lists 'Customer, Employee, OrderStatus, Book')

            // ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Email"); 
            // ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FirstName"); 
            // ViewData["OrderStatusId"] = new SelectList(_context.OrderStatuses, "OrderStatusId", "Name");

            return View(); // stub
        }

        // POST: Orders/Create
        // process form data to create a new order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order) // [Bind("...")] or ViewModel
        {
            // TODO: save an order and its OrderItems

            // if (ModelState.IsValid)
            // {
            //     _context.Add(order);
            //     await _context.SaveChangesAsync();
            //     return RedirectToAction(nameof(Index));
            // }

            // TODO: get data for ViewModel when validation fails
            // return View(order); 

            return RedirectToAction(nameof(Index)); // stub
        }

        // GET: Orders/Edit/5
        // display the order editing form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // TODO: receive an order for editing

            // var order = await _context.Orders.FindAsync(id);
            // if (order == null)
            // {
            //     return NotFound();
            // }

            // TODO: get data for ViewModel
            // return View(order);

            return View(new Order { Id = id.Value }); // stub
        }

        // POST: Orders/Edit/5
        // process form data for order editing
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order) // [Bind("...")] or ViewModel
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            // TODO: update an order and its OrderItems

            // if (ModelState.IsValid)
            // {
            //     try
            //     {
            //         _context.Update(order);
            //         await _context.SaveChangesAsync();
            //     }
            //     catch (DbUpdateConcurrencyException)
            //     {
            //         if (!OrderExists(order.OrderId)) { return NotFound(); }
            //         else { throw; }
            //     }
            //     return RedirectToAction(nameof(Index));
            // }

            // TODO: get data for ViewModel when validation fails
            // return View(order); 

            return RedirectToAction(nameof(Index)); // stub
        }

        // GET: Orders/Delete/5
        // display confirmation of order deletion
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // TODO: order details to confirm deletion

            // var order = await _context.Orders
            //     .Include(...) // load related data for display
            //     .FirstOrDefaultAsync(m => m.OrderId == id);
            // if (order == null)
            // {
            //     return NotFound();
            // }
            // return View(order);

            return View(new Order { Id = id.Value }); // stub
        }

        // POST: Orders/Delete/5
        // delete order 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // TODO: delete an order (and possibly linked OrderItems)

            // var order = await _context.Orders.FindAsync(id);
            // if (order != null)
            // {
            //     _context.Orders.Remove(order);
            // }
            // await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // helper method 
        private bool OrderExists(int id)
        {
            // TODO: check the existence of an order

            // return _context.Orders.Any(e => e.OrderId == id);

            return false; // stub
        }

    }
}
