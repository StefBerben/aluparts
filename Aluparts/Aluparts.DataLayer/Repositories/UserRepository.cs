using Aluparts.DataLayer.Entities;
using Aluparts.DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aluparts.DataLayer.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AlupartsDbContext _context;

    public UserRepository(AlupartsDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}