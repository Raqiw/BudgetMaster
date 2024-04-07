namespace BudgetMaster.IApplication.Models.DTO
{
    public class BudgetCalculationDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public float HourlyRate { get; set; }
        public float WorkedHours { get; set; }
        public int UserId { get; set; }
        public List<TransactionDto> Incomes { get; set; }
        public List<TransactionDto> Expenses { get; set; }
    }
}
