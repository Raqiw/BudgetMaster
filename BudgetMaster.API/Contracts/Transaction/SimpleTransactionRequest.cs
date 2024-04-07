namespace BudgetMaster.API.Contracts.Transaction
{
    public record SimpleTransactionRequest
    {
        public string Name { get; init; }
        public decimal Amount { get; init; }
    }
}
