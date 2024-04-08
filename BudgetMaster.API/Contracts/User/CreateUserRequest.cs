namespace BudgetMaster.API.Contracts.User
{
    public class CreateUserRequest
    {
        public long TelegramId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
