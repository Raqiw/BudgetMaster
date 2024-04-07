using BudgetMaster.TelegramBot.Concole.Models;
using BudgetMaster.TelegramBot.Concole.Services;
using Newtonsoft.Json;
using PRTelegramBot.Attributes;
using PRTelegramBot.Extensions;
using PRTelegramBot.Models;
using PRTelegramBot.Utils;
using System.Globalization;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
namespace BudgetMaster.TelegramBot.Concole.Handlers
{
    public class SlashCommandHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CommandService _commandService;
        private readonly MessageService _messageService;
        private readonly ReportService _reportService;

        public SlashCommandHandler(IHttpClientFactory httpClientFactory, CommandService commandService, MessageService messageService, ReportService reportService)
        {
            _httpClientFactory = httpClientFactory;
            _commandService = commandService;
            _messageService = messageService;
            _reportService = reportService;
        }

        [SlashHandler("/start")]
        public async Task CommandStart(ITelegramBotClient botClient, Update update)
        {
            var user = update.Message.From;

            var httpClient = _httpClientFactory.CreateClient("MyApiClient");

            var userRegistrationData = new
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                TelegramId = user.Id
            };

            string jsonUserData = JsonConvert.SerializeObject(userRegistrationData);

            HttpResponseMessage response = await httpClient.PostAsync("api/users", new StringContent(jsonUserData, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)                                                                                  //CheckUserExists
            {
                await HandleNewUser(botClient, update);
            }
            else
            {
                await HandleReturningUser(botClient, update);
            }
        }

        private async Task HandleReturningUser(ITelegramBotClient botClient, Update update)
        {
            var user = update.Message.From;
            var message =
                $"С возвращением, {user.FirstName}! Чем я могу вам помочь сегодня? Вы можете просмотреть предыдущие расчеты командой /viewbudgets или начать новый командой /newbudget.";
            await _messageService.SendMessageAsync(botClient, update, message);
        }

        private async Task HandleNewUser(ITelegramBotClient botClient, Update update)
        {
            var user = update.Message.From;
            var message =
                $"Добро пожаловать, {user.FirstName}! Я готов помочь вам с расчетом бюджета. Вы можете начать новый расчет командой /newbudget.";
            await _messageService.SendMessageAsync(botClient, update, message);
        }

        [SlashHandler("/viewbudgets")]
        public async Task CommandViewBudgets(ITelegramBotClient botClient, Update update)
        {
            var user = update.Message.From;

            var httpClient = _httpClientFactory.CreateClient("MyApiClient");

            HttpResponseMessage response = await httpClient.GetAsync($"api/budgetcalculations/user/{user.Id}");
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var budgetCalculations = JsonConvert.DeserializeObject<IEnumerable<BudgetCalculationForView>>(responseBody);
                await SendBudgetCalculations(botClient, update, budgetCalculations);
            }
            else
            {
                // Если запрос не успешен, отправляем сообщение об ошибке
                await _messageService.SendMessageAsync(botClient, update, "Не удалось получить список бюджетных расчетов. Попробуйте позже.");
            }
        }

        private async Task SendBudgetCalculations(ITelegramBotClient botClient, Update update, IEnumerable<BudgetCalculationForView> budgetCalculations)
        {
            if (budgetCalculations.Any())
            {
                var response = new StringBuilder("Your budget calculations:\n");
                foreach (var calculation in budgetCalculations)
                {
                    response.Append($"/viewbudget_{calculation.Id} {calculation.Title}\n");
                }
                await _messageService.SendMessageAsync(botClient, update, response.ToString());
            }
            else
            {
                await _messageService.SendMessageAsync(botClient, update, "У вас пока нет сохраненных бюджетных расчетов. Вы можете создать новый бюджет, используя команду /newbudget.");
            }
        }

        [SlashHandler("/viewbudget_")]
        public async Task CommandViewBudget(ITelegramBotClient botClient, Update update)
        {

            var budgetCalculationId = _commandService.ExtractBudgetCalculationId(update.Message.Text);

            if (budgetCalculationId != -1)
            {
                var httpClient = _httpClientFactory.CreateClient("MyApiClient");

                HttpResponseMessage response = await httpClient.GetAsync($"api/budgetcalculations/{budgetCalculationId}");

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var budgetCalculation = JsonConvert.DeserializeObject<BudgetCalculationForReport>(responseBody);
                    await _messageService.SendMessageAsync(botClient, update, await _reportService.GenerateReportAsync(budgetCalculation));
                }
                else
                {
                    // Если запрос не успешен, отправляем сообщение об ошибке
                    await _messageService.SendMessageAsync(botClient, update, "Ошибка. Такой расчет бюджета отсутствует.");
                }
            }
            else
            {
                await _messageService.SendMessageAsync(botClient, update, "Ошибка. Некорректная команда для просмотра расчета бюджета.");
            }
        }

        [SlashHandler("/newbudget")]
        public async Task CommandNewBudget(ITelegramBotClient botClient, Update update)
        {
            var titleCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DateTime.Today.AddMonths(1).ToString(format: "MMMM", new CultureInfo("ru-Ru")));

            var optionMessage = new OptionMessage();
            optionMessage.MenuReplyKeyboardMarkup = MenuGenerator.ReplyKeyboard(1, new List<string> { titleCase }, true, "Отмена");

            update.RegisterStepHandler(new StepTelegram(StepBudgetTitle_BudgetHourlyRate, new StepCache()));
            await _messageService.SendMessageAsync(botClient, update, "Давайте начнем создание нового бюджета. Укажите название для данного расчета бюджета.", optionMessage);
        }

        public async Task StepBudgetTitle_BudgetHourlyRate(ITelegramBotClient botClient, Update update)
        {
            var optionMessage = new OptionMessage();
            optionMessage.MenuReplyKeyboardMarkup = MenuGenerator.ReplyKeyboard(1, new List<string>(), true, "Отмена");

            if (_commandService.IsCancellationRequested(botClient, update))
            {
                return;
            }

            var handler = update.GetStepHandler<StepTelegram>();
            handler!.GetCache<StepCache>().BudgetTitle = update.Message.Text;

            var httpClient = _httpClientFactory.CreateClient("MyApiClient");
            HttpResponseMessage response = await httpClient.GetAsync($"api/users/telegram/{update.Message.From.Id}");

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<Models.User>(responseBody);
                handler!.GetCache<StepCache>().UserId = user.Id;
            }


            handler.RegisterNextStep(StepBudgetHourlyRate_BudgetWorkedHours, DateTime.Now.AddMinutes(5));
            await _messageService.SendMessageAsync(botClient, update, "Укажите вашу почасовую ставку (в формате 00,00).", optionMessage);
        }

        public async Task StepBudgetHourlyRate_BudgetWorkedHours(ITelegramBotClient botClient, Update update)
        {
            await HandleUserInput(botClient, update, async userInput =>
            {
                var handler = update.GetStepHandler<StepTelegram>();
                handler!.GetCache<StepCache>().BudgetHourlyRate = userInput;

                var optionMessage = new OptionMessage();
                optionMessage.MenuReplyKeyboardMarkup = MenuGenerator.ReplyKeyboard(1, new List<string>(), true, "Отмена");

                handler.RegisterNextStep(StepBudgetWorkedHours_Income, DateTime.Now.AddMinutes(5));
                await _messageService.SendMessageAsync(botClient, update, "Отлично! Теперь введите количество отработанных часов.", optionMessage);
            });
        }

        public async Task StepBudgetWorkedHours_Income(ITelegramBotClient botClient, Update update)
        {
            await HandleUserInput(botClient, update, async userInput =>
            {
                var handler = update.GetStepHandler<StepTelegram>();
                handler!.GetCache<StepCache>().BudgetWorkedHours = userInput;

                var optionMessage = new OptionMessage();
                optionMessage.MenuReplyKeyboardMarkup = MenuGenerator.ReplyKeyboard(2, new List<string> { "Добавить доходы", "Пропустить" }, true, "Отмена");

                handler.RegisterNextStep(StepAddIncome, DateTime.Now.AddMinutes(5));
                await _messageService.SendMessageAsync(botClient, update, "Теперь вы можете добавить дополнительные источники дохода или пропустить этот шаг.", optionMessage);
            });
        }

        public async Task StepAddIncome(ITelegramBotClient botClient, Update update)
        {
            var handler = update.GetStepHandler<StepTelegram>();
            var optionMessage = new OptionMessage();

            await HandleUserInput(botClient, update, async userInput =>
            {
                switch (userInput)
                {
                    case "добавить доходы":
                        optionMessage.MenuReplyKeyboardMarkup = MenuGenerator.ReplyKeyboard(1, new List<string>(), true, "Отмена");

                        handler.RegisterNextStep(StepAddIncomeTitle, DateTime.Now.AddMinutes(5));
                        await _messageService.SendMessageAsync(botClient, update, "Введите название дополнительного источника дохода:", optionMessage);
                        break;

                    case "пропустить":
                        optionMessage.MenuReplyKeyboardMarkup = MenuGenerator.ReplyKeyboard(2, new List<string> { "Добавить расходы", "Пропустить" }, true, "Отмена");

                        handler.RegisterNextStep(StepAddExpenses, DateTime.Now.AddMinutes(5));
                        await _messageService.SendMessageAsync(botClient, update, "Теперь вы можете ввести запланированные расходы или пропустить этот шаг.", optionMessage);
                        break;

                    default:
                        optionMessage.MenuReplyKeyboardMarkup = MenuGenerator.ReplyKeyboard(2, new List<string> { "Добавить доходы", "Пропустить" }, true, "Отмена");

                        await _messageService.SendMessageAsync(botClient, update, "Добавьте дополнительные источники дохода или пропустите этот шаг.", optionMessage);
                        return;
                }
            });
        }

        public async Task StepAddIncomeTitle(ITelegramBotClient botClient, Update update)
        {
            var optionMessage = new OptionMessage();
            optionMessage.MenuReplyKeyboardMarkup = MenuGenerator.ReplyKeyboard(1, new List<string>(), true, "Отмена");

            if (_commandService.IsCancellationRequested(botClient, update))
            {
                return;
            }

            var handler = update.GetStepHandler<StepTelegram>();
            handler!.GetCache<StepCache>().LastIncomeName = update.Message.Text;

            handler.RegisterNextStep(StepAddIncomeAmount, DateTime.Now.AddMinutes(5));
            await _messageService.SendMessageAsync(botClient, update, "Введите сумму для этого источника дохода:", optionMessage);
        }

        public async Task StepAddIncomeAmount(ITelegramBotClient botClient, Update update)
        {
            await HandleUserInput(botClient, update, async userInput =>
            {
                var handler = update.GetStepHandler<StepTelegram>();
                handler!.GetCache<StepCache>().LastIncomeAmount = userInput;
                handler!.GetCache<StepCache>().AdditionalIncomes.Add(new SimpleTransaction
                {
                    Name = handler!.GetCache<StepCache>().LastIncomeName,
                    Amount = handler!.GetCache<StepCache>().LastIncomeAmount
                });

                var optionMessage = new OptionMessage();
                optionMessage.MenuReplyKeyboardMarkup = MenuGenerator.ReplyKeyboard(2, new List<string> { "Да", "Нет" }, true, "Отмена");

                handler.RegisterNextStep(StepAdditionalIncomeDecision, DateTime.Now.AddMinutes(5));
                await _messageService.SendMessageAsync(botClient, update, "Источник дохода успешно добавлен.");
                await _messageService.SendMessageAsync(botClient, update, "Хотите добавить еще один источник дохода?", optionMessage);
            });
        }

        public async Task StepAdditionalIncomeDecision(ITelegramBotClient botClient, Update update)
        {
            var handler = update.GetStepHandler<StepTelegram>();
            var optionMessage = new OptionMessage();

            await HandleUserInput(botClient, update, async userInput =>
            {
                switch (userInput)
                {
                    case "да":
                        optionMessage.MenuReplyKeyboardMarkup = MenuGenerator.ReplyKeyboard(1, new List<string>(), true, "Отмена");

                        handler.RegisterNextStep(StepAddIncomeTitle, DateTime.Now.AddMinutes(5));
                        await _messageService.SendMessageAsync(botClient, update, "Введите название дополнительного источника дохода:");
                        break;

                    case "нет":
                        optionMessage.MenuReplyKeyboardMarkup = MenuGenerator.ReplyKeyboard(2, new List<string> { "Добавить расходы", "Пропустить" }, true, "Отмена");

                        handler.RegisterNextStep(StepAddExpenses, DateTime.Now.AddMinutes(5));
                        await _messageService.SendMessageAsync(botClient, update, "Теперь вы можете ввести запланированные расходы или пропустить этот шаг.", optionMessage);
                        break;

                    default:
                        optionMessage.MenuReplyKeyboardMarkup = MenuGenerator.ReplyKeyboard(2, new List<string> { "Да", "Нет" }, true, "Отмена");

                        await _messageService.SendMessageAsync(botClient, update, "Выберите Да или Нет.", optionMessage);
                        return;
                }
            });
        }

        public async Task StepAddExpenses(ITelegramBotClient botClient, Update update)
        {
            var handler = update.GetStepHandler<StepTelegram>();
            var optionMessage = new OptionMessage();

            await HandleUserInput(botClient, update, async userInput =>
            {
                switch (userInput)
                {
                    case "добавить расходы":
                        optionMessage.MenuReplyKeyboardMarkup = MenuGenerator.ReplyKeyboard(1, new List<string>(), true, "Отмена");

                        handler.RegisterNextStep(StepAddExpenseTitle, DateTime.Now.AddMinutes(5));
                        await _messageService.SendMessageAsync(botClient, update, "Введите название расхода:", optionMessage);
                        break;

                    case "пропустить":
                        await CreateBudgetCalculation(botClient, update);
                        update.ClearStepUserHandler();
                        break;

                    default:
                        optionMessage.MenuReplyKeyboardMarkup = MenuGenerator.ReplyKeyboard(2, new List<string> { "Добавить расходы", "Пропустить" }, true, "Отмена");

                        await _messageService.SendMessageAsync(botClient, update, "Добавьте расходы или пропустите этот шаг.", optionMessage);
                        return;
                }
            });
        }

        public async Task StepAddExpenseTitle(ITelegramBotClient botClient, Update update)
        {
            var optionMessage = new OptionMessage();
            optionMessage.MenuReplyKeyboardMarkup = MenuGenerator.ReplyKeyboard(1, new List<string>(), true, "Отмена");

            if (_commandService.IsCancellationRequested(botClient, update))
            {
                return;
            }

            var handler = update.GetStepHandler<StepTelegram>();
            handler!.GetCache<StepCache>().LastExpenseName = update.Message.Text;

            handler.RegisterNextStep(StepAddExpenseAmount, DateTime.Now.AddMinutes(5));
            await _messageService.SendMessageAsync(botClient, update, "Введите сумму для этого расхода:", optionMessage);
        }

        public async Task StepAddExpenseAmount(ITelegramBotClient botClient, Update update)
        {
            await HandleUserInput(botClient, update, async userInput =>
            {
                var handler = update.GetStepHandler<StepTelegram>();
                handler!.GetCache<StepCache>().LastExpenseAmount = userInput;
                handler!.GetCache<StepCache>().AdditionalExpenses.Add(new SimpleTransaction
                {
                    Name = handler!.GetCache<StepCache>().LastExpenseName,
                    Amount = handler!.GetCache<StepCache>().LastExpenseAmount
                });

                var optionMessage = new OptionMessage();
                optionMessage.MenuReplyKeyboardMarkup = MenuGenerator.ReplyKeyboard(2, new List<string> { "Да", "Нет" }, true, "Отмена");

                handler.RegisterNextStep(StepAdditionalExpenseDecision, DateTime.Now.AddMinutes(5));
                await _messageService.SendMessageAsync(botClient, update, "Расход успешно добавлен.");
                await _messageService.SendMessageAsync(botClient, update, "Хотите добавить еще один пункт расходов?", optionMessage);
            });
        }

        public async Task StepAdditionalExpenseDecision(ITelegramBotClient botClient, Update update)
        {
            var handler = update.GetStepHandler<StepTelegram>();
            var optionMessage = new OptionMessage();

            await HandleUserInput(botClient, update, async userInput =>
            {
                switch (userInput)
                {
                    case "да":
                        optionMessage.MenuReplyKeyboardMarkup = MenuGenerator.ReplyKeyboard(1, new List<string>(), true, "Отмена");

                        handler.RegisterNextStep(StepAddExpenseTitle, DateTime.Now.AddMinutes(5));
                        await _messageService.SendMessageAsync(botClient, update, "Введите название расхода:", optionMessage);
                        break;

                    case "нет":
                        await CreateBudgetCalculation(botClient, update);
                        update.ClearStepUserHandler();
                        break;

                    default:
                        optionMessage.MenuReplyKeyboardMarkup = MenuGenerator.ReplyKeyboard(2, new List<string> { "Да", "Нет" }, true, "Отмена");

                        await _messageService.SendMessageAsync(botClient, update, "Выберите Да или Нет.", optionMessage);
                        return;
                }
            });
        }

        private async Task HandleUserInput(ITelegramBotClient botClient, Update update, Func<string, Task> handleInput)
        {
            if (_commandService.IsCancellationRequested(botClient, update)) return;

            await handleInput(update.Message.Text.ToLowerInvariant());
        }

        public async Task HandleUserInput(ITelegramBotClient botClient, Update update, Func<decimal, Task> handleInput)
        {
            if (_commandService.IsCancellationRequested(botClient, update))
            {
                return;
            }

            if (!decimal.TryParse(update.Message.Text, out var userInput))
            {
                await _messageService.SendMessageAsync(botClient, update, "Неверный формат. Пожалуйста, введите число.");
                return;
            }

            await handleInput(userInput);
        }

        private async Task CreateBudgetCalculation(ITelegramBotClient botClient, Update update)
        {
            var handler = update.GetStepHandler<StepTelegram>();

            var budgetCalculationForCreate = new BudgetCalculationForCreate
            {
                Title = handler!.GetCache<StepCache>().BudgetTitle,
                HourlyRate = handler!.GetCache<StepCache>().BudgetHourlyRate,
                WorkedHours = handler!.GetCache<StepCache>().BudgetWorkedHours,
                UserId = handler!.GetCache<StepCache>().UserId,
                Incomes = handler!.GetCache<StepCache>().AdditionalIncomes.Select(income => new SimpleTransaction { Name = income.Name, Amount = income.Amount }).ToList(),
                Expenses = handler!.GetCache<StepCache>().AdditionalExpenses.Select(expense => new SimpleTransaction { Name = expense.Name, Amount = expense.Amount }).ToList(),
            };

            var httpClient = _httpClientFactory.CreateClient("MyApiClient");

            string jsonBudgetCalculationData = JsonConvert.SerializeObject(budgetCalculationForCreate);

            HttpResponseMessage response = await httpClient.PostAsync($"api/budgetcalculations", new StringContent(jsonBudgetCalculationData, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var budgetCalculationForReport = JsonConvert.DeserializeObject<BudgetCalculationForReport>(responseBody);
                var budgetReport = await _reportService.GenerateReportAsync(budgetCalculationForReport);

                await botClient.SendTextMessageAsync(update.Message.Chat.Id, budgetReport, replyMarkup: new ReplyKeyboardRemove());
            }
        }
    }
}
