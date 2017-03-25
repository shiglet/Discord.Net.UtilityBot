using Discord;
using Discord.Addons.InteractiveCommands;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Discord.Addons.SimpleInjectorBridge;
using SimpleInjector;
using UtilityBot.Services.Configuration;
using UtilityBot.Services.GitHub;
using UtilityBot.Services.Logging;
using UtilityBot.Services.Tags;

namespace UtilityBot
{
    class Program
    {
        static void Main(string[] args) =>
            new Program().RunAsync().GetAwaiter().GetResult();

        private DiscordSocketClient client;
        private Config config;
        private CommandHandler handler;

        private async Task RunAsync()
        {
            client = new DiscordSocketClient();
            config = Config.Load();

            var map = ConfigureServicesAsync();

            await client.LoginAsync(TokenType.Bot, config.Token);
            await client.StartAsync();

            handler = new CommandHandler(map);
            await handler.ConfigureAsync();

            await Task.Delay(-1);
        }

        private IDependencyMap ConfigureServicesAsync()
        {
            var container = new Container();
            container.RegisterSingleton(client);
            container.RegisterSingleton(config);
            container.RegisterSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false }));
            container.RegisterSingleton<LogService>();
            container.RegisterSingleton<InteractiveService>();
            container.RegisterSingleton<TagService>();
            container.RegisterSingleton<GitHubService>();
            // SimpleInjector will not instantiate a Singleton until the first request, so we need to request a few
            // of these now.
            // I don't plan on using SimpleInjector after this project for this reason.
            container.GetInstance<LogService>();
            container.GetInstance<TagService>();
            container.GetInstance<GitHubService>();
            return new SimpleDependencyMap(container);
        }
    }
}