using BudgetMaster.IPersistence.Models;

namespace BudgetMaster.IPersistence.Repositories
{
    public interface IIncomeRepository : ITransactionRepository<IncomeEntity> { }
}
