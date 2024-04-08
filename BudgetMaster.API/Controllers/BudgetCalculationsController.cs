using BudgetMaster.API.Contracts.BudgetCalculation;
using BudgetMaster.API.Contracts.Transaction;
using BudgetMaster.IApplication.Models.DTO;
using BudgetMaster.IApplication.Services;
using Microsoft.AspNetCore.Mvc;

namespace BudgetMaster.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetCalculationsController : ControllerBase
    {
        private readonly IBudgetCalculationService _budgetCalculationService;
        private readonly IUserService _userService;

        public BudgetCalculationsController(IBudgetCalculationService budgetCalculationService, IUserService userService)
        {
            _budgetCalculationService = budgetCalculationService;
            _userService = userService;
        }

        /// <summary>
        /// Creates a new budget calculation.
        /// </summary>
        /// <param name="request">The request to create a budget calculation.</param>
        /// <returns>The newly created budget calculation.</returns>
        /// <response code="200">Returns the newly created budget calculation.</response>
        /// <response code="404">Not Found. The resource was not found.</response>
        /// <response code="500">Internal Server Error. An unexpected error occurred.</response>
        [HttpPost]
        [ProducesResponseType(typeof(BudgetCalculationResponse), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> CreateBudgetCalculationAsync([FromBody] CreateBudgetCalculationRequest request)
        {
            var budgetCalculationToCreateDto = new BudgetCalculationToCreateDto
            {
                Title = request.Title,
                HourlyRate = request.HourlyRate,
                WorkedHours = request.WorkedHours,
                UserId = request.UserId,
                Expenses = request.Expenses.Select(ex => new SimpleTransactionDto { Name = ex.Name, Amount = ex.Amount }).ToList(),
                Incomes = request.Incomes.Select(ex => new SimpleTransactionDto { Name = ex.Name, Amount = ex.Amount }).ToList(),
            };

            var createBudgetCalculationResult = await _budgetCalculationService.CreateBudgetCalculationAsync(budgetCalculationToCreateDto);

            if (createBudgetCalculationResult.IsFailure)
            {
                return (createBudgetCalculationResult.Error.Contains("failed", StringComparison.OrdinalIgnoreCase))
                    ? StatusCode(StatusCodes.Status500InternalServerError, createBudgetCalculationResult.Error)
                    : NotFound(createBudgetCalculationResult.Error);
            }

            var createdCalculation = createBudgetCalculationResult.Value;

            var response = new BudgetCalculationResponse()
            {
                Id = createdCalculation.Id,
                Title = createdCalculation.Title,
                HourlyRate = createdCalculation.HourlyRate,
                WorkedHours = createdCalculation.WorkedHours,
                UserId = createdCalculation.UserId,
                Incomes = createdCalculation.Incomes
                .Select(transaction => new SimpleTransactionResponse
                {
                    Id = transaction.Id,
                    Name = transaction.Name,
                    Amount = transaction.Amount
                })
                .ToList(),
                Expenses = createdCalculation.Expenses
                .Select(transaction => new SimpleTransactionResponse
                {
                    Id = transaction.Id,
                    Name = transaction.Name,
                    Amount = transaction.Amount
                })
                .ToList(),
            };

            return Ok(response);
        }

        /// <summary>
        /// Retrieves a list of budget calculations for a user based on their Telegram ID.
        /// </summary>
        /// <param name="telegramId">The Telegram ID of the user.</param>
        /// <returns>The list of budget calculations for the user.</returns>
        /// <response code="200">OK. The budget calculations are successfully retrieved.</response>
        /// <response code="404">Not Found. No budget calculations found for the specified Telegram ID.</response>
        [HttpGet("user/{telegramId:long}")]
        [ProducesResponseType(typeof(IEnumerable<BudgetCalculationResponse>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> GetBudgetCalculationsByTelegramIdAsync(long telegramId)
        {
            var budgetCalculations = await _budgetCalculationService.GetBudgetCalculationsByTelegramIdAsync(telegramId);

            if( budgetCalculations == null)
            return NotFound();

            var response = budgetCalculations.
                Select(bc => new BudgetCalculationResponse()
                {
                    Id = bc.Id,
                    Title = bc.Title,
                    HourlyRate = bc.HourlyRate,
                    WorkedHours = bc.WorkedHours,
                    UserId = bc.UserId,
                    Expenses = bc.Expenses
                    .Select(e => new SimpleTransactionResponse()
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Amount = e.Amount
                    }).ToList(),
                    Incomes = bc.Incomes
                    .Select(e => new SimpleTransactionResponse()
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Amount = e.Amount
                    }).ToList()
                });
            return Ok(response);
        }

        /// <summary>
        /// Retrieves a budget calculation by its ID.
        /// </summary>
        /// <param name="id">The ID of the budget calculation.</param>
        /// <returns>The budget calculation.</returns>
        /// <response code="200">OK. The budget calculation is successfully retrieved.</response>
        /// <response code="404">Not Found. No budget calculation found for the specified ID.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(BudgetCalculationResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> GetBudgetCalculationByIdAsync(int id)
        {
            var budgetCalculationResult = await _budgetCalculationService.GetBudgetCalculationByIdAsync(id);
            if (budgetCalculationResult.IsFailure)
                return NotFound(budgetCalculationResult.Error);

            var response = new BudgetCalculationResponse
            {
                Id = budgetCalculationResult.Value.Id,
                Title = budgetCalculationResult.Value.Title,
                HourlyRate = budgetCalculationResult.Value.HourlyRate,
                WorkedHours = budgetCalculationResult.Value.WorkedHours,
                UserId = budgetCalculationResult.Value.UserId,
                Incomes = budgetCalculationResult.Value.Incomes.Select(i => new SimpleTransactionResponse()
                {
                    Id = i.Id,
                    Name = i.Name,
                    Amount = i.Amount
                }).ToList(),
                Expenses = budgetCalculationResult.Value.Expenses.Select(e => new SimpleTransactionResponse()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Amount = e.Amount,
                }).ToList()
            };
            return Ok(response);
        }

        /// <summary>
        /// Updates a budget calculation by its ID.
        /// </summary>
        /// <param name="id">The ID of the budget calculation to update.</param>
        /// <param name="request">The updated budget calculation request.</param>
        /// <response code="204">No Content. The budget calculation is successfully updated.</response>
        /// <response code="404">Not Found. No budget calculation found for the specified ID.</response>
        /// <response code="500">Bad Request. The provided ID does not match the ID in the request body.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]

        public async Task<IActionResult> UpdateBudgetCalculation(int id, [FromBody] UpdateBudgetCalculationRequest request)
        {
            var updateBudgetCalculationDto = new UpdateBudgetCalculationDto()
            {
                Id = request.Id,
                Title = request.Title,
                WorkedHours = request.WorkedHours,
                HourlyRate = request.HourlyRate,
                Incomes = request.Incomes.Select(incomeRequest => new UdpateTransactionDto()
                {
                    Id = incomeRequest.Id,
                    Name = incomeRequest.Name,
                    Amount = incomeRequest.Amount,
                }).ToList(),
                Expenses = request.Expenses.Select(expenceRequest => new UdpateTransactionDto()
                {
                    Id = expenceRequest.Id,
                    Name = expenceRequest.Name,
                    Amount = expenceRequest.Amount,
                }).ToList()
            };
            var updateBudgetCalculationResult = await _budgetCalculationService.UpdateBudgetCalculationAsync(id, updateBudgetCalculationDto);

            if (updateBudgetCalculationResult.IsFailure)
            {
                return (updateBudgetCalculationResult.Error.Contains("failed", StringComparison.OrdinalIgnoreCase))
                   ? StatusCode(StatusCodes.Status500InternalServerError, updateBudgetCalculationResult.Error)
                   : NotFound(updateBudgetCalculationResult.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a budget calculation by its ID.
        /// </summary>
        /// <param name="id">The ID of the budget calculation to delete.</param>
        /// <response code="204">No Content. The deletion is successful.</response>
        /// <response code="404">Not Found. No budget calculation found for the specified ID.</response>
        /// <response code="500">Bad Request. The provided ID does not match the ID in the request body.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteBudgetCalculation(int id)
        {
            var deleteBudgetCalculationResult = await _budgetCalculationService.DeleteBudgetCalculationAsync(id);

            if (deleteBudgetCalculationResult.IsFailure)
            {
                return (deleteBudgetCalculationResult.Error.Contains("failed", StringComparison.OrdinalIgnoreCase))
                    ? StatusCode(StatusCodes.Status500InternalServerError, deleteBudgetCalculationResult.Error)
                    : NotFound(deleteBudgetCalculationResult.Error);
            }

            return NoContent();
        }
    }
}
