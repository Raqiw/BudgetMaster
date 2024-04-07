using BudgetMaster.API.Contracts.Transaction;

namespace BudgetMaster.API.Contracts.BudgetCalculation
{
    public record BudgetCalculationResponse
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public float HourlyRate { get; init; }
        public float WorkedHours { get; init; }

        public int UserId { get; init; }
        public List<SimpleTransactionResponse> Incomes { get; init; }
        public List<SimpleTransactionResponse> Expenses { get; init; }
    }
}
