namespace BudgetMaster.API.Contracts.Transaction
{
    public record SimpleTransactionResponse
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public decimal Amount { get; init; }
    }
}
