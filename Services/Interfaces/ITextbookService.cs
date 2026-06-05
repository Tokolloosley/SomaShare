using SomaShare.Models;

namespace SomaShare.Services
{
    public interface ITextbookService
    {
        Task<List<Textbook>> GetAllAvailableBooksAsync();
        Task<Textbook?> GetByIdAsync(int id);
        Task AddAsync(Textbook textbook);
        Task UpdateAsync(Textbook textbook);
        Task DeleteAsync(int id);
    }
}