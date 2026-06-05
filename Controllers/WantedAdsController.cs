using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SomaShare.Data;
using SomaShare.Models;

namespace SomaShare.Controllers
{
    public class WantedAdsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WantedAdsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: WantedAds
        public async Task<IActionResult> Index()
        {
            var wantedAds = await _context.WantedAds
                .Include(w => w.User)
                .Where(w => w.IsActive)
                .ToListAsync();
            return View(wantedAds);
        }

        // GET: WantedAds/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: WantedAds/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(WantedAd wantedAd)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                wantedAd.UserId = user.Id;
                wantedAd.CreatedDate = DateTime.UtcNow;

                _context.Add(wantedAd);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(wantedAd);
        }

        // GET: WantedAds/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var wantedAd = await _context.WantedAds.FindAsync(id);
            if (wantedAd == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (wantedAd.UserId != user.Id && !User.IsInRole("Admin"))
                return Forbid();

            return View(wantedAd);
        }

        // POST: WantedAds/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, WantedAd wantedAd)
        {
            if (id != wantedAd.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wantedAd);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WantedAdExists(wantedAd.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(wantedAd);
        }

        // GET: WantedAds/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var wantedAd = await _context.WantedAds
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (wantedAd == null)
                return NotFound();

            return View(wantedAd);
        }

        // POST: WantedAds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wantedAd = await _context.WantedAds.FindAsync(id);
            _context.WantedAds.Remove(wantedAd);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WantedAdExists(int id)
        {
            return _context.WantedAds.Any(e => e.Id == id);
        }
    }
}