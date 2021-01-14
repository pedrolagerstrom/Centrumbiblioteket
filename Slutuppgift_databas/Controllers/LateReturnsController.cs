using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Slutuppgift_databas.Data;
using Slutuppgift_databas.Models;

namespace Slutuppgift_databas.Controllers
{
    public class LateReturnsController : Controller
    {
        private readonly LibraryContext _context;

        public LateReturnsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: LateReturns
        public async Task<IActionResult> Index()
        {
            var libraryContext = _context.Rentals.Where(r => r.ExpectedReturnDate < DateTime.Now).Include(r => r.Customer).Include(r => r.Inventory).Include(r => r.Inventory.Book);
            return View(await libraryContext.ToListAsync());
        }

        // GET: LateReturns/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Customer)
                .Include(r => r.Inventory)
                .FirstOrDefaultAsync(m => m.RentalId == id);
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // GET: LateReturns/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName");
            ViewData["InventoryId"] = new SelectList(_context.Inventory, "InventoryId", "InventoryId");
            return View();
        }

        // POST: LateReturns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RentalId,InventoryId,CustomerId,Rented,RentalDate,ReturnDate,ExpectedReturnDate")] Rental rental)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rental);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", rental.CustomerId);
            ViewData["InventoryId"] = new SelectList(_context.Inventory, "InventoryId", "InventoryId", rental.InventoryId);
            return View(rental);
        }

        // GET: LateReturns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", rental.CustomerId);
            ViewData["InventoryId"] = new SelectList(_context.Inventory, "InventoryId", "InventoryId", rental.InventoryId);
            return View(rental);
        }

        // POST: LateReturns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RentalId,InventoryId,CustomerId,Rented,RentalDate,ReturnDate,ExpectedReturnDate")] Rental rental)
        {
            if (id != rental.RentalId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rental);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalExists(rental.RentalId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", rental.CustomerId);
            ViewData["InventoryId"] = new SelectList(_context.Inventory, "InventoryId", "InventoryId", rental.InventoryId);
            return View(rental);
        }

        // GET: LateReturns/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Customer)
                .Include(r => r.Inventory)
                .FirstOrDefaultAsync(m => m.RentalId == id);
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // POST: LateReturns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);
            //_context.Rentals.Remove(rental);
            //await _context.SaveChangesAsync();

            _context.Entry(rental).State = EntityState.Modified;
            rental.ReturnDate = DateTime.Now;
            rental.Rented = "No";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool RentalExists(int id)
        {
            return _context.Rentals.Any(e => e.RentalId == id);
        }
    }
}
