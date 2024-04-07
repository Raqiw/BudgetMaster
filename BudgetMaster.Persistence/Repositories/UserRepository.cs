using BudgetMaster.IPersistence.Models;
using BudgetMaster.IPersistence.Repositories;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace BudgetMaster.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BudgetMasterDbContext _dbContext;

        public UserRepository(BudgetMasterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserEntity> GetUserByIdAsync(int userId) =>
            await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

        public async Task<UserEntity> GetUserByTelegramIdAsync(long telegramId) =>
            await _dbContext.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId);

        public async Task<Result> AddUserAsync(UserEntity user)
        {
            try
            {
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to create user: {ex.Message}");
            }
        }
    }
}