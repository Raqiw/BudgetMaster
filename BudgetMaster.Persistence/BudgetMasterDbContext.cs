using BudgetMaster.IPersistence.Models;
using BudgetMaster.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BudgetMaster.Persistence
{
        public class BudgetMasterDbContext: DbContext
        {
            public BudgetMasterDbContext(DbContextOptions<BudgetMasterDbContext> options) : base(options) { }

            public DbSet<UserEntity> Users { get; set; }
            public DbSet<BudgetCalculationEntity> BudgetCalculations { get; set; }
            public DbSet<IncomeEntity> Incomes { get; set; }
            public DbSet<ExpenseEntity> Expenses { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.ApplyConfiguration(new BudgetCalculationConficuration());
                modelBuilder.ApplyConfiguration(new UserConficuration());
                modelBuilder.ApplyConfiguration(new IncomeConficuration());
                modelBuilder.ApplyConfiguration(new ExpenseConficuration());

                base.OnModelCreating(modelBuilder);
            }
        }
}
