using Aluparts.DataLayer.Entities;

namespace Aluparts.DataLayer.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}