using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SomaShare.Data;
using SomaShare.Models;

namespace SomaShare.Controllers
{
    [Authorize]
    public class OffersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OffersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // VIEW USER OFFERS
 
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var offers = await _context.Offers
                .Include(o => o.Textbook)
                .Include(o => o.Buyer)
                .Include(o => o.Seller)
                .Where(o =>
                    o.BuyerId == user.Id ||
                    o.SellerId == user.Id)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();

            return View(offers);
        }

        // CREATE OFFER
        public async Task<IActionResult> Create(int textbookId)
        {
            var textbook =
                await _context.Textbooks.FindAsync(textbookId);

            if (textbook == null)
                return NotFound();

            if (!textbook.IsAvailable)
            {
                TempData["Error"] =
                    "This textbook is no longer available.";

                return RedirectToAction(
                    "Details",
                    "Textbooks",
                    new { id = textbookId });
            }

            var offer = new Offer
            {
                TextbookId = textbook.Id,
                SellerId = textbook.SellerId,
                OfferPrice = textbook.AskingPrice
            };

            return View(offer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Offer offer)
        {
            var user = await _userManager.GetUserAsync(User);

            offer.BuyerId = user.Id;
            offer.CreatedDate = DateTime.UtcNow;
            offer.Status = "Pending";

            if (!ModelState.IsValid)
                return View(offer);

            _context.Offers.Add(offer);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Offer submitted successfully.";

            return RedirectToAction(nameof(Index));
        }

        // ACCEPT OFFER
        [HttpPost]
        [Authorize(Roles = "Seller,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            var offer = await _context.Offers
                .Include(o => o.Textbook)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (offer == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            if (offer.SellerId != user.Id &&
                !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (offer.Status != "Pending")
            {
                TempData["Error"] =
                    "This offer has already been processed.";

                return RedirectToAction(nameof(Index));
            }

            offer.Status = "Accepted";
            offer.ResponseDate = DateTime.UtcNow;

            var transaction = new Transaction
            {
                OfferId = offer.Id,
                BuyerId = offer.BuyerId,
                SellerId = offer.SellerId,
                FinalPrice = offer.OfferPrice,
                TransactionDate = DateTime.UtcNow,
                Status = "Pending"
            };

            _context.Transactions.Add(transaction);

            if (offer.Textbook != null)
            {
                offer.Textbook.IsAvailable = false;
                _context.Textbooks.Update(offer.Textbook);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Offer accepted successfully.";

            return RedirectToAction(
                "Details",
                "Textbooks",
                new { id = offer.TextbookId });
        }

        // REJECT OFFER

        [HttpPost]
        [Authorize(Roles = "Seller,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var offer = await _context.Offers
                .FirstOrDefaultAsync(o => o.Id == id);

            if (offer == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            if (offer.SellerId != user.Id &&
                !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (offer.Status != "Pending")
            {
                TempData["Error"] =
                    "This offer has already been processed.";

                return RedirectToAction(nameof(Index));
            }

            offer.Status = "Rejected";
            offer.ResponseDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Offer rejected successfully.";

            return RedirectToAction(
                "Details",
                "Textbooks",
                new { id = offer.TextbookId });
        }
        // OFFER DETAILS
  
        public async Task<IActionResult> Details(int id)
        {
            var offer = await _context.Offers
                .Include(o => o.Textbook)
                .Include(o => o.Buyer)
                .Include(o => o.Seller)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (offer == null)
                return NotFound();

            return View(offer);
        }
    }
}