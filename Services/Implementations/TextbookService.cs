using Microsoft.EntityFrameworkCore;
using SomaShare.Data;
using SomaShare.Models;

namespace SomaShare.Services
{
    public class TextbookService : ITextbookService
    {
        private readonly ApplicationDbContext _context;

        public TextbookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Textbook>> GetAllAvailableBooksAsync()
        {
            return await _context.Textbooks
                .Where(t => t.IsAvailable)
                .ToListAsync();
        }

        public async Task<Textbook?> GetByIdAsync(int id)
        {
            return await _context.Textbooks
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(Textbook textbook)
        {
            _context.Textbooks.Add(textbook);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Textbook textbook)
        {
            _context.Textbooks.Update(textbook);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var book = await _context.Textbooks.FindAsync(id);

            if (book != null)
            {
                _context.Textbooks.Remove(book);
                await _context.SaveChangesAsync();
            }
        }
    }
}