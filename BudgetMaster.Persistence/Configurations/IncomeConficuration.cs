using BudgetMaster.IPersistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetMaster.Persistence.Configurations
{
    public class IncomeConficuration : IEntityTypeConfiguration<IncomeEntity>
    {
        public void Configure(EntityTypeBuilder<IncomeEntity> builder)
        {
            builder.HasKey(bc => bc.Id);

            builder
                .HasOne(i => i.BudgetCalculation)
                .WithMany(bc=>bc.Incomes)
                .HasForeignKey(i=>i.BudgetCalculationId);
        }
    }
}
