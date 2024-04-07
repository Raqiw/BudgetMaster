namespace BudgetMaster.IApplication.Models.DTO
{
    public class TelegramUserDto
    {
        public int Id { get; set; }
        public long TelegramId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
