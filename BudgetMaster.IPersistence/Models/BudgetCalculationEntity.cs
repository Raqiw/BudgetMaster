namespace BudgetMaster.IPersistence.Models
{
    public class BudgetCalculationEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public float HourlyRate { get; set; } = 0;
        public float WorkedHours { get; set; } = 0;
        public DateTime DateCreated { get; set; }

        public virtual int UserId { get; set; }
        public virtual UserEntity? User { get; set; }
        public virtual List<IncomeEntity> Incomes { get; set; } = [];
        public virtual List<ExpenseEntity> Expenses { get; set; } = [];
    }
}
