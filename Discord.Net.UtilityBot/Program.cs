using Discord;
using Discord.Addons.InteractiveCommands;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
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

            var serviceProvider = ConfigureServicesAsync();

            await _client.LoginAsync(TokenType.Bot, _config.Token);
            await _client.StartAsync();

            _handler = new CommandHandler(serviceProvider);
            await _handler.ConfigureAsync();

            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServicesAsync()
        {
            var services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_config)
                .AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false, ThrowOnError = false}))
                .AddSingleton(LogAdaptor.CreateLogger())
                .AddSingleton<LogAdaptor>()
                .AddSingleton<InteractiveService>()
                .AddSingleton<TagService>()
                .AddSingleton<GitHubService>();
            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);
            // Autowire and create these dependencies now
            provider.GetService<LogAdaptor>();
            provider.GetService<TagService>();
            provider.GetService<GitHubService>();
            return provider;
        }
    }
}