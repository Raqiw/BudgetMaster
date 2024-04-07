namespace BudgetMaster.IApplication.Models.DTO
{
    public class UpdateBudgetCalculationDto
    {
        public int Id { get; set; }
        public string Title { get; init; }
        public float HourlyRate { get; init; }
        public float WorkedHours { get; init; }

        public List<UdpateTransactionDto> Incomes { get; init; }
        public List<UdpateTransactionDto> Expenses { get; init; }
    }
}
