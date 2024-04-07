namespace BudgetMaster.IApplication.Models.DTO
{
    public class UserDto
    {
        public int Id { get; set; }
        public long TelegramId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual List<BudgetCalculationDto> BudgetCalculations { get; set; }

    }
}
