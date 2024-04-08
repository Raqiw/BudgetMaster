using BudgetMaster.TelegramBot.Concole.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PRTelegramBot.Configs;
using PRTelegramBot.Core;
using PRTelegramBot.Extensions;

namespace BudgetMaster.TelegramBot.Concole
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            var configuration = builder.Configuration;

            builder.Services.AddSingletonBotHandlers();

            builder.Services.AddHttpClient("MyApiClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7095/");
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            builder.Services.AddScoped<CommandService>();
            builder.Services.AddScoped<MessageService>();
            builder.Services.AddScoped<ReportService>();

            void PrBotInstance_OnLogError(Exception ex, long? id)
            {
                Console.WriteLine(ex.ToString());
            }

            void PrBotInstance_OnLogCommon(string msg, Enum typeEvent, ConsoleColor color)
            {
                Console.WriteLine(msg);
            }

            configuration.AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true);

            using var host = builder.Build();

            var telegramBotToken = configuration.GetSection("TelegramBotConfig").Get<TelegramConfig>().Token;

            var prBotInstance = new PRBot(new TelegramConfig
            {
                Token = telegramBotToken,
                ClearUpdatesOnStart = true,
                BotId = 0,
            },
                host.Services.GetService<IServiceProvider>()
            );

            prBotInstance.OnLogCommon += PrBotInstance_OnLogCommon;
            prBotInstance.OnLogError += PrBotInstance_OnLogError;
            await prBotInstance.Start();

            await host.RunAsync();
        }
    }
}
