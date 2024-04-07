using BudgetMaster.IPersistence.Models;

namespace BudgetMaster.IPersistence.Repositories
{
    public interface IExpenseRepository : ITransactionRepository<ExpenseEntity> { }
}
