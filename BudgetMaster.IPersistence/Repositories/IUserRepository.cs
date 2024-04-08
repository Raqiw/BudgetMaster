using BudgetMaster.IPersistence.Models;
using CSharpFunctionalExtensions;

namespace BudgetMaster.IPersistence.Repositories
{
    public interface IUserRepository
    {
        Task<UserEntity> GetUserByIdAsync(int userId);
        Task<UserEntity> GetUserByTelegramIdAsync(long telegramId);
        Task<Result> AddUserAsync(UserEntity user);
    }
}
