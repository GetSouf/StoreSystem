using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreSystem.Models;
using testproject.Models;

namespace StoreSystem.Controllers
{
    public class ProductSuppliersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductSuppliersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductSuppliers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ProductSuppliers.Include(p => p.Product).Include(p => p.Supplier);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ProductSuppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productSupplier = await _context.ProductSuppliers
                .Include(p => p.Product)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productSupplier == null)
            {
                return NotFound();
            }

            return View(productSupplier);
        }

        // GET: ProductSuppliers/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name");
            return View();
        }

        // POST: ProductSuppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,SupplierId,SupplyPrice")] ProductSupplier productSupplier)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productSupplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productSupplier.ProductId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", productSupplier.SupplierId);
            return View(productSupplier);
        }

        // GET: ProductSuppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productSupplier = await _context.ProductSuppliers.FindAsync(id);
            if (productSupplier == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productSupplier.ProductId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", productSupplier.SupplierId);
            return View(productSupplier);
        }

        // POST: ProductSuppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,SupplierId,SupplyPrice")] ProductSupplier productSupplier)
        {
            if (id != productSupplier.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productSupplier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductSupplierExists(productSupplier.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productSupplier.ProductId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", productSupplier.SupplierId);
            return View(productSupplier);
        }

        // GET: ProductSuppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productSupplier = await _context.ProductSuppliers
                .Include(p => p.Product)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productSupplier == null)
            {
                return NotFound();
            }

            return View(productSupplier);
        }

        // POST: ProductSuppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productSupplier = await _context.ProductSuppliers.FindAsync(id);
            if (productSupplier != null)
            {
                _context.ProductSuppliers.Remove(productSupplier);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductSupplierExists(int id)
        {
            return _context.ProductSuppliers.Any(e => e.Id == id);
        }
    }
}
