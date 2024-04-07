namespace BudgetMaster.TelegramBot.Concole.Models
{
    public class BudgetCalculationForReport
    {
        public string Title { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal WorkedHours { get; set; }

        public List<SimpleTransaction> Incomes { get; set; }
        public List<SimpleTransaction> Expenses { get; set; }
    }
}
