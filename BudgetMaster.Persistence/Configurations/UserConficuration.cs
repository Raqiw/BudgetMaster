using BudgetMaster.IPersistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetMaster.Persistence.Configurations
{
    public class UserConficuration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasKey(bc => bc.Id);

            builder.HasIndex(u => u.TelegramId).IsUnique();

            builder
                .HasMany(u => u.BudgetCalculations)
                .WithOne(bc => bc.User)
                .HasForeignKey(bc => bc.UserId);
        }
    }
}
