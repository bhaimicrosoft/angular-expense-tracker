using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    private readonly AppDbContext _context = context;


    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // AsNoTracking improves performance for read-only operations at DB level
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        // AsNoTracking improves performance for read-only operations at DB level
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        // write operation
        await _context.Users.AddAsync(user, cancellationToken);
        //save changes
        await _context.SaveChangesAsync(cancellationToken);
    }
}