using BudgetMaster.IPersistence.Models;
using BudgetMaster.IPersistence.Repositories;
using Microsoft.EntityFrameworkCore;


namespace BudgetMaster.Persistence.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly BudgetMasterDbContext _dbContext;

        public ExpenseRepository(BudgetMasterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ExpenseEntity> GetByIdAsync(int id)
        {
            return await _dbContext.Expenses.FirstAsync(ex => ex.Id == id);
        }

        public async Task<List<ExpenseEntity>> GetByBudgetCalculationIdAsync(int budgetCalculationId)
        {
            return await _dbContext
                .Expenses
                .Where(e => e.BudgetCalculation.Id == budgetCalculationId)
                .ToListAsync();
        }

        public async Task<bool> AddAsync(ExpenseEntity expense)
        {
            _dbContext.Expenses.Add(expense);
            return await _dbContext.SaveChangesAsync() != 0;
        }
    }
}
