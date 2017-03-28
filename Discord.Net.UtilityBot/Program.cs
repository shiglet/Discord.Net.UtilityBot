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
    internal class Program
    {
        private static void Main(string[] args) =>
            new Program().RunAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private Config _config;
        private CommandHandler _handler;

        private async Task RunAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
#if DEBUG
                LogLevel = LogSeverity.Debug,
#else
                LogLevel = LogSeverity.Verbose,
#endif
            });
            _config = Config.Load();

            var map = ConfigureServicesAsync();

            await _client.LoginAsync(TokenType.Bot, _config.Token);
            await _client.StartAsync();

            _handler = new CommandHandler(map);
            await _handler.ConfigureAsync();

            await Task.Delay(-1);
        }

        private IDependencyMap ConfigureServicesAsync()
        {
            var container = new Container();
            container.RegisterSingleton(_client);
            container.RegisterSingleton(_config);
            container.RegisterSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false, ThrowOnError = false}));
            container.RegisterSingleton(LogAdaptor.CreateLogger());
            container.RegisterSingleton<LogAdaptor>();
            container.RegisterSingleton<InteractiveService>();
            container.RegisterSingleton<TagService>();
            container.RegisterSingleton<GitHubService>();
            // Autowire and create these dependencies now
            container.GetInstance<LogAdaptor>();
            container.GetInstance<TagService>();
            container.GetInstance<GitHubService>();
            return new SimpleDependencyMap(container);
        }
    }
}