using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SomaShare.Data;
using SomaShare.Models;

namespace SomaShare.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reviews/Create/5
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
                return NotFound();

            var transaction = await _context.Transactions
                .Include(t => t.Offer)
                .ThenInclude(o => o.Textbook)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (transaction.BuyerId != user.Id && !User.IsInRole("Admin"))
                return Forbid();

            if (transaction.Review != null)
                return BadRequest("Review already exists for this transaction");

            var review = new Review { TransactionId = transaction.Id, ReviewedUserId = transaction.SellerId };
            return View(review);
        }

        // POST: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionId,ReviewedUserId,Rating,Comment")] Review review)
        {
            if (ModelState.IsValid)
            {
                var transaction = await _context.Transactions.FindAsync(review.TransactionId);
                if (transaction == null)
                    return NotFound();

                var user = await _userManager.GetUserAsync(User);
                if (transaction.BuyerId != user.Id && !User.IsInRole("Admin"))
                    return Forbid();

                review.ReviewerUserId = user.Id;
                review.ReviewDate = DateTime.UtcNow;

                _context.Add(review);
                await _context.SaveChangesAsync();

                // Update user's average rating
                await UpdateUserRating(review.ReviewedUserId);

                return RedirectToAction("MyTransactions", "Transactions");
            }

            return View(review);
        }

        // GET: Reviews/UserReviews/id
        public async Task<IActionResult> UserReviews(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var reviews = await _context.Reviews
                .Include(r => r.ReviewerUser)
                .Include(r => r.Transaction)
                .Where(r => r.ReviewedUserId == id)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            ViewData["ReviewedUser"] = user;
            return View(reviews);
        }

        private async Task UpdateUserRating(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var reviews = await _context.Reviews
                .Where(r => r.ReviewedUserId == userId)
                .ToListAsync();

            if (reviews.Any())
            {
                user.AverageRating = reviews.Average(r => r.Rating);
                user.TotalTransactions = reviews.Count;
                await _userManager.UpdateAsync(user);
            }
        }
    }
}