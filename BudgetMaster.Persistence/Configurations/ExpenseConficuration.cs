using BudgetMaster.IPersistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetMaster.Persistence.Configurations
{
    public class ExpenseConficuration : IEntityTypeConfiguration<ExpenseEntity>
    {
        public void Configure(EntityTypeBuilder<ExpenseEntity> builder)
        {
            builder.HasKey(bc => bc.Id);

            builder
                .HasOne(ex => ex.BudgetCalculation)
                .WithMany(bc => bc.Expenses)
                .HasForeignKey(ex => ex.BudgetCalculationId);
        }
    }
}
