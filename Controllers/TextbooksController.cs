using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SomaShare.Data;
using SomaShare.Models;
using SomaShare.ViewModels;
using SomaShare.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SomaShare.Controllers
{
    public class TextbooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TextbooksController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Textbooks (Search + Filter + Pagination + Sort)
        public async Task<IActionResult> Index(
            string searchTerm,
            string author,
            string condition,
            decimal? minPrice,
            decimal? maxPrice,
            string sortOrder,
            int page = 1)
        {
            int pageSize = 10;

            IQueryable<Textbook> query = _context.Textbooks
                .Include(t => t.Seller)
                .Where(t => t.IsAvailable);

            // Search
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(t =>
                    t.Title.Contains(searchTerm) ||
                    t.Author.Contains(searchTerm) ||
                    t.ISBN.Contains(searchTerm) ||
                    t.CourseCode.Contains(searchTerm));
            }

            // Filter Author
            if (!string.IsNullOrWhiteSpace(author))
            {
                query = query.Where(t => t.Author.Contains(author));
            }

            // Filter Condition
            if (!string.IsNullOrWhiteSpace(condition))
            {
                query = query.Where(t => t.Condition == condition);
            }

            // Price Filters
            if (minPrice.HasValue)
                query = query.Where(t => t.AskingPrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(t => t.AskingPrice <= maxPrice.Value);

            // Sorting
            query = sortOrder switch
            {
                "price_asc" => query.OrderBy(t => t.AskingPrice),
                "price_desc" => query.OrderByDescending(t => t.AskingPrice),
                "title_desc" => query.OrderByDescending(t => t.Title),
                "newest" => query.OrderByDescending(t => t.ListedDate),
                _ => query.OrderBy(t => t.Title)
            };

            int totalItems = await query.CountAsync();

            var textbooks = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.Author = author;
            ViewBag.Condition = condition;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SortOrder = sortOrder;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(textbooks);
        }

        // GET: Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var textbook = await _context.Textbooks
                .Include(t => t.Seller)
                .Include(t => t.Offers)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (textbook == null) return NotFound();

            return View(textbook);
        }

        // GET: Create
        [Authorize(Roles = "Seller,Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create (WITH IMAGE UPLOAD)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> Create(Textbook textbook)
        {
            if (!ModelState.IsValid)
                return View(textbook);

            var user = await _userManager.GetUserAsync(User);

            textbook.SellerId = user.Id;
            textbook.ListedDate = DateTime.UtcNow;
            textbook.IsAvailable = true;

            // ✅ IMAGE UPLOAD ADDED
            if (textbook.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/images");

                Directory.CreateDirectory(uploadsFolder);

                string fileName = Guid.NewGuid() +
                                  Path.GetExtension(textbook.ImageFile.FileName);

                string filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await textbook.ImageFile.CopyToAsync(stream);

                textbook.ImageUrl = "/images/" + fileName;
            }

            _context.Add(textbook);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Textbook listed successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Edit
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var textbook = await _context.Textbooks.FindAsync(id);

            if (textbook == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);

            if (textbook.SellerId != user.Id && !User.IsInRole("Admin"))
                return Forbid();

            return View(textbook);
        }

        // POST: Edit (WITH IMAGE UPLOAD)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> Edit(int id, Textbook textbook)
        {
            if (id != textbook.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(textbook);

            try
            {
                var existingBook = await _context.Textbooks.FindAsync(id);

                if (existingBook == null)
                    return NotFound();

                existingBook.Title = textbook.Title;
                existingBook.Author = textbook.Author;
                existingBook.ISBN = textbook.ISBN;
                existingBook.Description = textbook.Description;
                existingBook.Edition = textbook.Edition;
                existingBook.AskingPrice = textbook.AskingPrice;
                existingBook.Condition = textbook.Condition;

                // ✅ IMAGE UPLOAD ADDED (EDIT VERSION)
                if (textbook.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/images");

                    Directory.CreateDirectory(uploadsFolder);

                    string fileName = Guid.NewGuid() +
                                      Path.GetExtension(textbook.ImageFile.FileName);

                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await textbook.ImageFile.CopyToAsync(stream);

                    existingBook.ImageUrl = "/images/" + fileName;
                }

                _context.Update(existingBook);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Textbook updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TextbookExists(textbook.Id))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Delete
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var textbook = await _context.Textbooks
                .Include(t => t.Seller)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (textbook == null) return NotFound();

            return View(textbook);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var textbook = await _context.Textbooks.FindAsync(id);

            if (textbook == null)
                return NotFound();

            _context.Textbooks.Remove(textbook);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Textbook deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private bool TextbookExists(int id)
        {
            return _context.Textbooks.Any(e => e.Id == id);
        }
    }
}