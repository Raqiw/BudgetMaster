using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using Telegram.Bot.Types;
using PRTelegramBot.Extensions;

namespace BudgetMaster.TelegramBot.Concole.Services
{
    public class CommandService
    {
        public bool IsCancellationRequested(ITelegramBotClient botClient, Update update)
        {
            if (update.Message.Text.ToLowerInvariant() != "отмена") return false;

            botClient.SendTextMessageAsync(update.Message.Chat.Id, "Процесс создания нового бюджета был прерван.", replyMarkup: new ReplyKeyboardRemove());
            update.ClearStepUserHandler();
            return true;
        }

        public int ExtractBudgetCalculationId(string messageText)
        {
            var commandParts = messageText.Split("_");
            return commandParts.Length > 1 && int.TryParse(commandParts[1], out var id) ? id : -1;
        }
    }
}