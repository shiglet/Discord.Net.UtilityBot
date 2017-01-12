using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using UtilityBot.Services.Configuration;
using UtilityBot.Services.Logging;
using UtilityBot.Services.Tags;

namespace UtilityBot
{
    class Program
    {
        static void Main(string[] args) =>
            new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient client;
        private Configuration config;
        private CommandHandler handler;

        private async Task Start()
        {
            client = new DiscordSocketClient();
            config = Configuration.Load();

            var map = new DependencyMap();
            ConfigureServices(map);

            await client.LoginAsync(TokenType.Bot, config.Token);
            await client.ConnectAsync();

            handler = new CommandHandler(map);
            await handler.Configure();

            await Task.Delay(-1);
        }

        private void ConfigureServices(DependencyMap map)
        {
            map.Add(client);
            map.Add(config);
            map.Add(new LogService(map));
            map.Add(new TagService(map));
        }
    }
}