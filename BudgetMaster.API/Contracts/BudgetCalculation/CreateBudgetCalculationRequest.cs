using BudgetMaster.API.Contracts.Transaction;

namespace BudgetMaster.API.Contracts.BudgetCalculation
{
    public record CreateBudgetCalculationRequest
    {
        public string Title { get; init; }
        public float HourlyRate { get; init; }
        public float WorkedHours { get; init; }

        public int UserId { get; init; }
        public List<SimpleTransactionRequest> Incomes { get; init; }
        public List<SimpleTransactionRequest> Expenses { get; init; }
    }
}
