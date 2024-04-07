namespace BudgetMaster.IApplication.Models.DTO
{
    public class BudgetCalculationToCreateDto
    {
        public string Title { get; set; }
        public float HourlyRate { get; set; }
        public float WorkedHours { get; set; }
        public int UserId { get; set; }
        public List<SimpleTransactionDto> Incomes { get; set; }
        public List<SimpleTransactionDto> Expenses { get; set; }
    }
}
