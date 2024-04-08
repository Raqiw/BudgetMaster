using BudgetMaster.IApplication.Models.DTO;
using CSharpFunctionalExtensions;

namespace BudgetMaster.IApplication.Services
{
    public interface IBudgetCalculationService
    {
        Task<Result<BudgetCalculationDto>> GetBudgetCalculationByIdAsync(int id);
        Task<IEnumerable<BudgetCalculationDto>> GetBudgetCalculationsByTelegramIdAsync(long telegramId);
        Task<Result<BudgetCalculationDto>> CreateBudgetCalculationAsync(BudgetCalculationToCreateDto budgetCalculationToCreateDto);
        Task<Result> DeleteBudgetCalculationAsync(int id);
        Task<Result> UpdateBudgetCalculationAsync(int id, UpdateBudgetCalculationDto updateBudgetCalculationDto);
    }
}
