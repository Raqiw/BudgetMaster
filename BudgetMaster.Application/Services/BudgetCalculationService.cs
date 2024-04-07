using BudgetMaster.IApplication.Models.DTO;
using BudgetMaster.IApplication.Services;
using BudgetMaster.IPersistence.DataModels;
using BudgetMaster.IPersistence.Models;
using BudgetMaster.IPersistence.Repositories;
using CSharpFunctionalExtensions;

namespace BudgetMaster.Application.Services
{
    public class BudgetCalculationService : IBudgetCalculationService
    {

        private readonly IBudgetCalculationRepository _budgetCalculationRepository;

        public BudgetCalculationService(IBudgetCalculationRepository budgetCalculationRepository)
        {
            _budgetCalculationRepository = budgetCalculationRepository;
        }

        public async Task<Result<BudgetCalculationDto>> CreateBudgetCalculationAsync(BudgetCalculationToCreateDto budgetCalculationToCreateDto)
        {
            var createBudgetCalculationDataModel = new CreateBudgetCalculationDataModel
            {
                Title = budgetCalculationToCreateDto.Title,
                HourlyRate = budgetCalculationToCreateDto.HourlyRate,
                WorkedHours = budgetCalculationToCreateDto.WorkedHours,
                UserId = budgetCalculationToCreateDto.UserId,
                Incomes = budgetCalculationToCreateDto.Incomes.Select(i => new SimpleTransactionDataModel
                {
                    Name = i.Name,
                    Amount = i.Amount,
                }).ToList(),
                Expenses = budgetCalculationToCreateDto.Expenses.Select(i => new SimpleTransactionDataModel
                {
                    Name = i.Name,
                    Amount = i.Amount,
                }).ToList(),
            };
            var resultCreateBudgetCalculation = await _budgetCalculationRepository.AddBudgetCalculationAsync(createBudgetCalculationDataModel);

            if (resultCreateBudgetCalculation.IsSuccess)
            {
                var budgetCalculationEntity = resultCreateBudgetCalculation.Value;
                var budgetCalculationDto = new BudgetCalculationDto()
                {
                    Id = budgetCalculationEntity.Id,
                    Title = budgetCalculationEntity.Title,
                    HourlyRate = budgetCalculationEntity.HourlyRate,
                    WorkedHours = budgetCalculationEntity.WorkedHours,
                    UserId = budgetCalculationEntity.UserId,
                    Incomes = budgetCalculationEntity.Incomes.Select(incomeEntity => new TransactionDto()
                    {
                        Id = incomeEntity.Id,
                        Name = incomeEntity.Name,
                        Amount = incomeEntity.Amount,
                    }).ToList(),
                    Expenses = budgetCalculationEntity.Expenses.Select(expenceEntity => new TransactionDto()
                    {
                        Id = expenceEntity.Id,
                        Name = expenceEntity.Name,
                        Amount = expenceEntity.Amount,
                    }).ToList()
                };

                return Result.Success(budgetCalculationDto);
            }

            return Result.Failure<BudgetCalculationDto>(resultCreateBudgetCalculation.Error);
        }

        public async Task<Result<BudgetCalculationDto>> GetBudgetCalculationByIdAsync(int id)
        {
            var budgetCalculationEntity = await _budgetCalculationRepository.GetBudgetCalculationByIdAsync(id);

            if (budgetCalculationEntity is null)
                return Result.Failure<BudgetCalculationDto>("Budget calculation not found.");

            return Result.Success(new BudgetCalculationDto
            {
                Id = budgetCalculationEntity.Id,
                Title = budgetCalculationEntity.Title,
                HourlyRate = budgetCalculationEntity.HourlyRate,
                WorkedHours = budgetCalculationEntity.WorkedHours,
                UserId = budgetCalculationEntity.UserId,
                Incomes = budgetCalculationEntity.Incomes.Select(incomeEntity => new TransactionDto()
                {
                    Id = incomeEntity.Id,
                    Name = incomeEntity.Name,
                    Amount = incomeEntity.Amount,
                }).ToList(),
                Expenses = budgetCalculationEntity.Expenses.Select(expenceEntity => new TransactionDto()
                {
                    Id = expenceEntity.Id,
                    Name = expenceEntity.Name,
                    Amount = expenceEntity.Amount,
                }).ToList(),
            });
        }

        public async Task<IEnumerable<BudgetCalculationDto>> GetBudgetCalculationsByTelegramIdAsync(long telegramId)
        {
            var budgetCalculationEntities = await _budgetCalculationRepository.GetBudgetCalculationsByTelegramIdAsync(telegramId);

            return budgetCalculationEntities.Select(budgetCalculationEntity => new BudgetCalculationDto
            {
                Id = budgetCalculationEntity.Id,
                Title = budgetCalculationEntity.Title,
                HourlyRate = budgetCalculationEntity.HourlyRate,
                WorkedHours = budgetCalculationEntity.WorkedHours,
                UserId = budgetCalculationEntity.UserId,
                Incomes = budgetCalculationEntity.Incomes.Select(incomeEntity => new TransactionDto()
                {
                    Id = incomeEntity.Id,
                    Name = incomeEntity.Name,
                    Amount = incomeEntity.Amount,
                }).ToList(),
                Expenses = budgetCalculationEntity.Expenses.Select(expenceEntity => new TransactionDto()
                {
                    Id = expenceEntity.Id,
                    Name = expenceEntity.Name,
                    Amount = expenceEntity.Amount,
                }).ToList(),
            }).ToList();
        }

        public async Task<Result> UpdateBudgetCalculationAsync(int id, UpdateBudgetCalculationDto updateBudgetCalculationDto)
        {
            var budgetCalculation = await _budgetCalculationRepository.GetBudgetCalculationByIdAsync(id);
            if (budgetCalculation is null)
            {
                return Result.Failure("Budget calculation not found.");
            }

            budgetCalculation.Title = updateBudgetCalculationDto.Title;
            budgetCalculation.WorkedHours = updateBudgetCalculationDto.WorkedHours;
            budgetCalculation.HourlyRate = updateBudgetCalculationDto.HourlyRate;

            foreach (var incomeDto in updateBudgetCalculationDto.Incomes)
            {
                if (incomeDto.Id.HasValue)
                {
                    var existingIncome = budgetCalculation.Incomes.FirstOrDefault(i => i.Id == incomeDto.Id);
                    if (existingIncome != null)
                    {
                        existingIncome.Name = incomeDto.Name;
                        existingIncome.Amount = incomeDto.Amount;
                    }
                    else
                    {
                        return Result.Failure($"Income with id {incomeDto.Id} not found.");
                    }
                }
                else
                {
                    budgetCalculation.Incomes.Add(new IncomeEntity()
                    {
                        Name = incomeDto.Name,
                        Amount = incomeDto.Amount,
                    });
                }
            }

            foreach (var expenseDto in updateBudgetCalculationDto.Expenses)
            {
                if (expenseDto.Id.HasValue)
                {
                    var existingIncome = budgetCalculation.Incomes.FirstOrDefault(i => i.Id == expenseDto.Id);
                    if (existingIncome is not null)
                    {
                        existingIncome.Name = expenseDto.Name;
                        existingIncome.Amount = expenseDto.Amount;
                    }
                    else
                    {
                        return Result.Failure($"Expense with id {expenseDto.Id} not found.");
                    }
                }
                else
                {
                    budgetCalculation.Expenses.Add(new ExpenseEntity()
                    {
                        Name = expenseDto.Name,
                        Amount = expenseDto.Amount,
                    });
                }
            }

            var updateBudgetCalculationResult = await _budgetCalculationRepository.UpdateBudgetCalculationAsync(budgetCalculation);
            if (updateBudgetCalculationResult.IsFailure)
            {
                return Result.Failure(updateBudgetCalculationResult.Error);
            }

            return Result.Success();
        }

        public async Task<Result> DeleteBudgetCalculationAsync(int id)
        {
            var budgetCalculation = await _budgetCalculationRepository.GetBudgetCalculationByIdAsync(id);
            if (budgetCalculation is null)
            {
                return Result.Failure("Budget calculation not found.");
            }

            var deleteBudgetCalculationResult = await _budgetCalculationRepository.DeleteBudgetCalculationAsync(budgetCalculation);

            if (deleteBudgetCalculationResult.IsFailure)
            {
                return Result.Failure(deleteBudgetCalculationResult.Error);
            }

            return Result.Success();
        }
    }
}
