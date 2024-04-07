using BudgetMaster.IPersistence.DataModels;
using BudgetMaster.IPersistence.Models;
using BudgetMaster.IPersistence.Repositories;
using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;
using Microsoft.EntityFrameworkCore;

namespace BudgetMaster.Persistence.Repositories
{
    public class BudgetCalculationRepository : IBudgetCalculationRepository
    {
        private readonly BudgetMasterDbContext _dbContext;
        private readonly IUserRepository _userRepository;

        public BudgetCalculationRepository(BudgetMasterDbContext dbContext, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
        }

        public async Task<Result<BudgetCalculationEntity>> AddBudgetCalculationAsync(CreateBudgetCalculationDataModel budgetCalculation)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var user = await _userRepository.GetUserByIdAsync(budgetCalculation.UserId);
                    if (user is null)
                    {
                        return Result.Failure<BudgetCalculationEntity>("User not found.");
                    }

                    var budgetCalculationEntity = new BudgetCalculationEntity
                    {
                        Title = budgetCalculation.Title,
                        HourlyRate = budgetCalculation.HourlyRate,
                        WorkedHours = budgetCalculation.WorkedHours,
                        DateCreated = DateTime.UtcNow,
                        UserId = budgetCalculation.UserId,
                        User = user
                    };

                    _dbContext.BudgetCalculations.Add(budgetCalculationEntity);
                    await _dbContext.SaveChangesAsync();

                    foreach (var incomeRequest in budgetCalculation.Incomes)
                    {
                        var incomeEntity = new IncomeEntity()
                        {
                            Name = incomeRequest.Name,
                            Amount = incomeRequest.Amount,
                            BudgetCalculationId = budgetCalculationEntity.Id,
                            BudgetCalculation = budgetCalculationEntity
                        };
                        _dbContext.Incomes.Add(incomeEntity);
                    }

                    foreach (var expenceRequest in budgetCalculation.Expenses)
                    {
                        var expenseEntity = new ExpenseEntity()
                        {
                            Name = expenceRequest.Name,
                            Amount = expenceRequest.Amount,
                            BudgetCalculationId = budgetCalculationEntity.Id,
                            BudgetCalculation = budgetCalculationEntity
                        };
                        _dbContext.Expenses.Add(expenseEntity);
                    }

                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();

                    return Result.Success(budgetCalculationEntity);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Result.Failure<BudgetCalculationEntity>($"Failed to create budget calculation. {ex.Message}");
                }
            }
        }

        public async Task<BudgetCalculationEntity?> GetBudgetCalculationByIdAsync(int budgetCalculationId) =>
             await _dbContext
            .BudgetCalculations
            .AsNoTracking()
            .Include(bc => bc.Incomes.OrderByDescending(incl => incl.Amount))
            .Include(bc => bc.Expenses.OrderByDescending(ex => ex.Amount))
            .Include(bc => bc.User)
            .FirstOrDefaultAsync(b => b.Id == budgetCalculationId);

        public async Task<List<BudgetCalculationEntity>> GetBudgetCalculationsByTelegramIdAsync(long telegramId) =>
             await _dbContext
            .BudgetCalculations
            .AsNoTracking()
            .Include(bc => bc.Incomes.OrderByDescending(incl => incl.Amount))
            .Include(bc => bc.Expenses.OrderByDescending(ex => ex.Amount))
            .Include(bc => bc.User)
            .OrderBy(bc => bc.DateCreated)
            .Where(bc => bc.User.TelegramId == telegramId)
            .ToListAsync();

        public async Task<Result> DeleteBudgetCalculationAsync(BudgetCalculationEntity budgetCalculation)
        {
            try
            {
                _dbContext.BudgetCalculations.Remove(budgetCalculation);
                await _dbContext.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to delete budget calculation: {ex.Message}");
            }
        }

        public async Task<Result> UpdateBudgetCalculationAsync(BudgetCalculationEntity budgetCalculation)
        {
            try
            {
                _dbContext.Update(budgetCalculation);
                await _dbContext.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to update budget calculation: {ex.Message}");
            }
        }
    }
}
