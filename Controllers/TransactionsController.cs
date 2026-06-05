using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SomaShare.Data;

namespace SomaShare.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var transactions = await _context.Transactions
                .Include(t => t.Buyer)
                .Include(t => t.Seller)
                .Include(t => t.Offer)
                .ToListAsync();

            return View(transactions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            var transaction =
                await _context.Transactions.FindAsync(id);

            if (transaction == null)
                return NotFound();

            transaction.Status = "Completed";

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Transaction completed successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}