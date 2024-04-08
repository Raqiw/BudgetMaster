using BudgetMaster.API.Contracts.Transaction;

namespace BudgetMaster.API.Contracts.BudgetCalculation
{
    public record UpdateBudgetCalculationRequest
    {
        public int Id { get; set; }
        public string Title { get; init; }
        public float HourlyRate { get; init; }
        public float WorkedHours { get; init; }

        public List<UdpateTransactionRequest> Incomes { get; init; }
        public List<UdpateTransactionRequest> Expenses { get; init; }
    }
}
