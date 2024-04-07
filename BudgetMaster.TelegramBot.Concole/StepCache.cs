using BudgetMaster.TelegramBot.Concole.Models;
using PRTelegramBot.Interface;
using System.Globalization;

namespace BudgetMaster.TelegramBot
{
    public class StepCache : ITelegramCache
    {
        public StepCache()
        {
            InitData();
        }

        public int UserId { get; set; }
        public string? BudgetTitle { get; set; }
        public decimal BudgetHourlyRate { get; set; }
        public decimal BudgetWorkedHours { get; set; }
        public string LastExpenseName { get; set; }
        public decimal LastExpenseAmount { get; set; }
        public string LastIncomeName { get; set; }
        public decimal LastIncomeAmount { get; set; }
        public List<SimpleTransaction> AdditionalIncomes { get; set; }
        public List<SimpleTransaction> AdditionalExpenses { get; set; }





        public void InitData()
        {
            BudgetTitle = DateTime.Today.AddMonths(1).Month.ToString("MMMM", new CultureInfo("ru-RU"));
            AdditionalIncomes = new List<SimpleTransaction>();
            AdditionalExpenses = new List<SimpleTransaction>();
        }

        public bool ClearData()
        {
            BudgetTitle = string.Empty;
            BudgetHourlyRate = default;
            BudgetWorkedHours = default;
            LastExpenseName = string.Empty;
            LastExpenseAmount = default;
            LastIncomeName = string.Empty;
            LastIncomeAmount = default;
            AdditionalIncomes.Clear();
            AdditionalExpenses.Clear();

            return true;
        }
    }
}
