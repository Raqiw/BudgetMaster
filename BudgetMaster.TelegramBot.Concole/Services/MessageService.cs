using PRTelegramBot.Models;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace BudgetMaster.TelegramBot.Concole.Services
{
    public class MessageService
    {
        public async Task SendMessageAsync(ITelegramBotClient botClient, Update update, string message, OptionMessage optionMessage = null)
        {
            if (optionMessage != null)
            {
                await PRTelegramBot.Helpers.Message.Send(botClient, update, message, optionMessage);
            }
            else
            {
                await PRTelegramBot.Helpers.Message.Send(botClient, update, message);
            }
        }
    }
}
