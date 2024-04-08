namespace BudgetMaster.API.Contracts.User
{
    public class UserResponse
    {
        public int Id { get; set; }
        public long TelegramId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
