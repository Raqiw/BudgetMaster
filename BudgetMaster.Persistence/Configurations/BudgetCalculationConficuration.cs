using BudgetMaster.IPersistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetMaster.Persistence.Configurations
{
    public class BudgetCalculationConficuration : IEntityTypeConfiguration<BudgetCalculationEntity>
    {
        public void Configure(EntityTypeBuilder<BudgetCalculationEntity> builder)
        {
            builder.HasKey(bc => bc.Id);

            builder
                .HasOne(bc => bc.User)
                .WithMany(u => u.BudgetCalculations)
                .HasForeignKey(bc => bc.UserId);

            builder
                .HasMany(bc => bc.Incomes)
                .WithOne(i => i.BudgetCalculation)
                .HasForeignKey(i => i.BudgetCalculationId);

            builder
                .HasMany(bc => bc.Expenses)
                .WithOne(ex => ex.BudgetCalculation)
                .HasForeignKey(ex => ex.BudgetCalculationId);
        }
    }
}
