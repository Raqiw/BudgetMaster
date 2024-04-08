using BudgetMaster.IPersistence.DataModels;
using BudgetMaster.IPersistence.Models;
using CSharpFunctionalExtensions;

namespace BudgetMaster.IPersistence.Repositories
{
    public interface IBudgetCalculationRepository
    {
        Task<Result<BudgetCalculationEntity>> AddBudgetCalculationAsync(CreateBudgetCalculationDataModel createBudgetCalculationDataModel);
        Task<BudgetCalculationEntity> GetBudgetCalculationByIdAsync(int budgetCalculationId);
        Task<List<BudgetCalculationEntity>> GetBudgetCalculationsByTelegramIdAsync(long telegramId);
        Task<Result> DeleteBudgetCalculationAsync(BudgetCalculationEntity budgetCalculation);
        Task<Result> UpdateBudgetCalculationAsync(BudgetCalculationEntity budgetCalculation);
    }
}
