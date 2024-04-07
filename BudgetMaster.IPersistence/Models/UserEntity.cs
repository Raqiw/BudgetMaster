namespace BudgetMaster.IPersistence.Models
{
    public class UserEntity
    {
        public int Id { get; set; }
        public long TelegramId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual List<BudgetCalculationEntity> BudgetCalculations { get; set; } = [];
    }
}
