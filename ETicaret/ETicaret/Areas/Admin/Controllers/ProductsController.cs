using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ETicaret.Data;
using ETicaret.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace ETicaret.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =Diger.Role_Admin)]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _he;
        public ProductsController(ApplicationDbContext context, IWebHostEnvironment he)
        {
            _context = context;
            _he = he;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                var file = HttpContext.Request.Form.Files;
                if (file.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var upload = Path.Combine(_he.WebRootPath, @"images\product");
                    var ext = Path.GetExtension(file[0].FileName);
                    if (product.Image != null)
                    {
                        var videoPath = Path.Combine(_he.WebRootPath, product.Image.TrimStart('\\'));
                        if (System.IO.File.Exists(videoPath))
                        {
                            System.IO.File.Delete(videoPath);
                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(upload, fileName + ext), FileMode.Create))
                    {
                        file[0].CopyTo(fileStreams);
                    }
                    product.Image = @"\images\product\" + fileName + ext;
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {

            if (ModelState.IsValid)
            {
                var file = HttpContext.Request.Form.Files;
                if (file.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var upload = Path.Combine(_he.WebRootPath, @"images\product");
                    var ext = Path.GetExtension(file[0].FileName);
                    if (product.Image != null)
                    {
                        var videoPath = Path.Combine(_he.WebRootPath, product.Image.TrimStart('\\'));
                        if (System.IO.File.Exists(videoPath))
                        {
                            System.IO.File.Delete(videoPath);
                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(upload, fileName + ext), FileMode.Create))
                    {
                        file[0].CopyTo(fileStreams);
                    }
                    product.Image = @"\images\product\" + fileName + ext;
                }
 
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
