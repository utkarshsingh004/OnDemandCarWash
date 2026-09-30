using Microsoft.EntityFrameworkCore;
using WasherService.Data;
using WasherService.Models;

namespace WasherService.Repositories
{
    public class WasherRepository : IWasherRepository
    {
        private readonly WasherDbContext _context;

        public WasherRepository(WasherDbContext context)
        {
            _context = context;
        }

        public async Task<List<Washer>> GetAllAsync()
        {
            return await _context.Washers.ToListAsync();
        }

        public async Task<Washer?> GetByIdAsync(Guid id)
        {
            return await _context.Washers
                .FirstOrDefaultAsync(w => w.WasherId == id);
        }

        public async Task<Washer?> GetByEmailAsync(string email)
        {
            return await _context.Washers
                .FirstOrDefaultAsync(w => w.Email == email);
        }

        public async Task AddAsync(Washer washer)
        {
            await _context.Washers.AddAsync(washer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Washer washer)
        {
            _context.Washers.Update(washer);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Washer>> GetAvailableAsync()
        {
            return await _context.Washers
                .Where(w => w.IsAvailable)
                .ToListAsync();
        }

        public async Task DeleteAsync(Washer washer)
        {
            _context.Washers.Remove(washer);
            await _context.SaveChangesAsync();
        }
    }
}