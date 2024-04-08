using BudgetMaster.IPersistence.Models;

namespace BudgetMaster.IPersistence.Repositories
{
    public interface ITransactionRepository<TValue> where TValue : TransactionEntity
    {
        Task<TValue> GetByIdAsync(int id);
        Task<List<TValue>> GetByBudgetCalculationIdAsync(int budgetCalculationId);
        Task<bool> AddAsync(TValue model);
    }
}
