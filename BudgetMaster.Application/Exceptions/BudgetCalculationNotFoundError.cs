namespace BudgetMaster.Application.Exceptions
{
    public class BudgetCalculationNotFoundError : Exception
    {
        public BudgetCalculationNotFoundError(string message) : base(message) { }
    }
}
