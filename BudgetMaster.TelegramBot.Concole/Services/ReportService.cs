using BudgetMaster.TelegramBot.Concole.Models;
using System.Text;

namespace BudgetMaster.TelegramBot.Concole.Services
{
    public class ReportService
    {
        public async Task<string> GenerateReportAsync(BudgetCalculationForReport budgetCalculationForReport)
        {
            var report = new StringBuilder();

            report.AppendLine($"Отчет по расчету бюджета \"{budgetCalculationForReport.Title}\"");
            report.AppendLine($"Почасовая оплата: {budgetCalculationForReport.HourlyRate}");
            report.AppendLine($"Количество отработанных часов за предыдущий месяц: {budgetCalculationForReport.WorkedHours:f1}");

            var salary = budgetCalculationForReport.WorkedHours * budgetCalculationForReport.HourlyRate;
            report.AppendLine($"Размер заработной платы за отработанные часы: {salary:f2}");

            report.AppendLine("\nДополнительные доходы:");
            foreach (var income in budgetCalculationForReport.Incomes)
            {
                report.AppendLine($"{income.Name}: {income.Amount}");
            }
            var totalIncome = budgetCalculationForReport.Incomes.Sum(i => i.Amount);
            var salaryPlusIncome = salary + totalIncome;
            report.AppendLine($"Размер дохода с учетом дополнительных источников: {salaryPlusIncome:f2}");

            report.AppendLine("\nРасходы:");
            foreach (var expense in budgetCalculationForReport.Expenses)
            {
                salaryPlusIncome -= expense.Amount;
                report.AppendLine($"{expense.Name}: {expense.Amount} | Остаток: {salaryPlusIncome:f2}");
            }

            report.AppendLine($"Размер остатка и учетом запланированных расходов: {salaryPlusIncome:f2}");

            return report.ToString();
        }
    }
}
