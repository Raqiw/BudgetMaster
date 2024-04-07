using BudgetMaster.IApplication.Models.DTO;
using BudgetMaster.IApplication.Services;
using BudgetMaster.IPersistence.Models;
using BudgetMaster.IPersistence.Repositories;
using CSharpFunctionalExtensions;

namespace BudgetMaster.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result> CreateUserAsync(UserToCreateDto userToCreateDto)
        {
            var existingUser = await _userRepository.GetUserByTelegramIdAsync(userToCreateDto.TelegramId);
            if (existingUser is not null)
            {
                return Result.Failure($"User with TelegramId {existingUser.TelegramId} already exists.");
            }

            var userEntity = new UserEntity()
            {
                TelegramId = userToCreateDto.TelegramId,
                FirstName = userToCreateDto.FirstName,
                LastName = userToCreateDto.LastName,
                DateCreated = DateTime.UtcNow,
            };

            var createUserResult = await _userRepository.AddUserAsync(userEntity);

            if (createUserResult.IsFailure)
            {
                return Result.Failure(createUserResult.Error);
            }

            //var createdUserDto = new TelegramUserDto()
            //{
            //    Id = createUserResult.Value.Id,
            //    TelegramId = createUserResult.Value.TelegramId,
            //    FirstName = createUserResult.Value.FirstName,
            //    LastName = createUserResult.Value.LastName,
            //};
            return Result.Success();
        }

        public async Task<Result<UserDto>> GetUserByTelegramIdAsync(long telegramId)
        {
            var userEntity = await _userRepository.GetUserByTelegramIdAsync(telegramId);
            if (userEntity == null)
            {
                return Result.Failure<UserDto>("User not found.");
            }
            return Result.Success(new UserDto()
            {
                Id = userEntity.Id,
                TelegramId = userEntity.TelegramId,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                BudgetCalculations = userEntity.BudgetCalculations.Select(bc => new BudgetCalculationDto()
                {
                    Id = bc.Id,
                    Title = bc.Title,
                    HourlyRate = bc.HourlyRate,
                    WorkedHours = bc.WorkedHours,
                    UserId = userEntity.Id,
                    Expenses = bc.Expenses.Select(expense => new TransactionDto()
                    {
                        Id = expense.Id,
                        Name = expense.Name,
                        Amount = expense.Amount
                    }).ToList(),
                    Incomes = bc.Incomes.Select(income => new TransactionDto()
                    {
                        Id = income.Id,
                        Name = income.Name,
                        Amount = income.Amount
                    }).ToList(),
                }).ToList()
            });
        }
    }
}
