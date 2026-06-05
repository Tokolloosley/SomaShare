using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SomaShare.Data;

namespace SomaShare.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            ViewBag.TotalUsers =
                _context.Users.Count();

            ViewBag.TotalTextbooks =
                _context.Textbooks.Count();

            ViewBag.TotalOffers =
                _context.Offers.Count();

            ViewBag.TotalTransactions =
                _context.Transactions.Count();

            ViewBag.TotalReviews =
                _context.Reviews.Count();

            ViewBag.TotalWantedAds =
                _context.WantedAds.Count();

            return View();
        }
    }
}