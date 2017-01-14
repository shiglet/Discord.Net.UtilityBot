using Discord;
using Discord.Addons.InteractiveCommands;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using UtilityBot.Services.Configuration;
using UtilityBot.Services.GitHub;
using UtilityBot.Services.Logging;
using UtilityBot.Services.Tags;

namespace UtilityBot
{
    class Program
    {
        static void Main(string[] args) =>
            new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient client;
        private Config config;
        private CommandHandler handler;

        private async Task Start()
        {
            client = new DiscordSocketClient();
            config = Config.Load();

            var map = new DependencyMap();
            await ConfigureServices(map);

            await client.LoginAsync(TokenType.Bot, config.Token);
            await client.ConnectAsync();

            handler = new CommandHandler(map);
            await handler.Configure();

            await Task.Delay(-1);
        }

        private async Task ConfigureServices(DependencyMap map)
        {
            map.Add(client);
            map.Add(config);
            map.Add(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false }));
            map.Add(new LogService(map));
            map.Add(new InteractiveService(client));
            await map.UsingTagService();
            map.Add(new GitHubService(map));
        }
    }
}