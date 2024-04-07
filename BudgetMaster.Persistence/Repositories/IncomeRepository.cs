using BudgetMaster.IPersistence.Models;
using BudgetMaster.IPersistence.Repositories;
using BudgetMaster.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BudgetMaster.Core.Data.Repositories
{
    public class IncomeRepository : IIncomeRepository
    {
        private readonly BudgetMasterDbContext _dbContext;

        public IncomeRepository(BudgetMasterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IncomeEntity> GetByIdAsync(int incomeId) =>
            await _dbContext.Incomes.FirstAsync(inc => inc.Id == incomeId);

        public async Task<List<IncomeEntity>> GetByBudgetCalculationIdAsync(int budgetCalculationId) =>
            await _dbContext
                .Incomes
                .Where(i => i.BudgetCalculation.Id == budgetCalculationId)
                .ToListAsync();

        public async Task<bool> AddAsync(IncomeEntity income)
        {
            _dbContext.Incomes.Add(income);
            return await _dbContext.SaveChangesAsync() != 0;
        }
    }
}
