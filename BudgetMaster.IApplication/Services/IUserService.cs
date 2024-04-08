using BudgetMaster.IApplication.Models.DTO;
using CSharpFunctionalExtensions;

namespace BudgetMaster.IApplication.Services
{
    public interface IUserService
    {
        public Task<Result> CreateUserAsync(UserToCreateDto userToCreateDto);
        public Task<Result<UserDto>> GetUserByTelegramIdAsync(long telegramId);
    }
}
