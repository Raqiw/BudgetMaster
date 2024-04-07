namespace BudgetMaster.TelegramBot.Concole.Models
{
    public class User
    {
        public int Id { get; set; }
        public long TelegramId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
