namespace BudgetMaster.IPersistence.DataModels
{
    public class CreateBudgetCalculationDataModel
    {
        public string Title { get; set; }
        public float HourlyRate { get; set; }
        public float WorkedHours { get; set; }
        public int UserId { get; set; }
        public List<SimpleTransactionDataModel> Incomes { get; set; }
        public List<SimpleTransactionDataModel> Expenses { get; set; }
    }
}
