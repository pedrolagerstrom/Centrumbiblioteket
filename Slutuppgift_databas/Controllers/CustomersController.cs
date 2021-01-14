using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slutuppgift_databas.Data;
using Slutuppgift_databas.Models;

namespace Slutuppgift_databas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly LibraryContext _context;

        public CustomersController(LibraryContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Customers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Customer>> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        [HttpPost("{customerId}/rentBook/{bookId}")]
        public async Task<ActionResult<Customer>> RentBook(int customerId, int bookId)
        {
            var customer = await _context.Customers
                .SingleOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return BadRequest("Customer does not exist!");
            }

            
            var inventory = await _context.Inventory
                .Include(i => i.Book)
                .Include(i => i.Rentals)
                .Where(i => i.BookId == bookId)
                .ToListAsync();
            
            var availableInv = inventory.FirstOrDefault(i => i.Available);

            if (availableInv == null)
            {
                return BadRequest("The book is not in stock.");
            }

            var rental = new Rental()
            {
                CustomerId = customerId,
                InventoryId = availableInv.InventoryId,
                RentalDate = DateTime.Now.AddDays(-60),
                ExpectedReturnDate = DateTime.Now.AddDays(-30),
                Rented = "Yes"
            };

            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();

            return Ok("Customer rented a book");
        }


        [HttpPost("{customerId}/returnBook/{bookId}")]
        public async Task<ActionResult<Customer>> ReturnBook(int customerId, int bookId)
        {            
            var customer = await _context.Customers
                .Include(c => c.Rentals)
                .ThenInclude(r => r.Inventory)
                .ThenInclude(i => i.Book)
                .SingleOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return BadRequest("Customer does not exist!");
            }

            if (customer.Rentals == null || customer.Rentals.Count == 0)
            {
                return BadRequest("Customer does not have any rentals!");
            }

            
            var rental = customer.Rentals.FirstOrDefault(r => r.Inventory.BookId == bookId && !r.Returned);

            if (rental == null)
            {
                return BadRequest("Customer have not rented this book.");
            }
            
            _context.Entry(rental).State = EntityState.Modified;
            rental.ReturnDate = DateTime.Now;
            rental.Rented = "No";

            await _context.SaveChangesAsync();

            return Ok("Customer returned the book");
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
