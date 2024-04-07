namespace BudgetMaster.IPersistence.Models
{
    public class TransactionEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; } = decimal.Zero;

        public int BudgetCalculationId { get; set; }
        public virtual BudgetCalculationEntity? BudgetCalculation { get; set; }
    }
}
