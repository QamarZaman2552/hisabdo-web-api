using HisabDo.Application.Repositories;
using HisabDo.Domain.Entities;
using HisabDo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HisabDo.Infrastructure.Repositories;

public class UserRepository(HisabDoDbContext context) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && !u.IsDeleted);
    }

    public async Task<User?> GetByIdAsync(int userId)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await context.Users
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await context.Users
            .Where(u => !u.IsDeleted)
            .OrderBy(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<User> AddAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int userId)
    {
        var user = await context.Users.FindAsync(userId);
        if (user != null)
        {
            user.IsDeleted = true;
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }
    }
}
