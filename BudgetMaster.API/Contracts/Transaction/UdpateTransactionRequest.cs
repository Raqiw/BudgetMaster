namespace BudgetMaster.API.Contracts.Transaction
{
    public class UdpateTransactionRequest
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }
}
