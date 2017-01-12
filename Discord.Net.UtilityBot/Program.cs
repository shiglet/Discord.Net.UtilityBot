using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using UtilityBot.Services.Configuration;

namespace UtilityBot
{
    class Program
    {
        static void Main(string[] args) =>
            new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient client;
        private Configuration config;

        private async Task Start()
        {
            client = new DiscordSocketClient();
            config = Configuration.Load();

            await client.LoginAsync(TokenType.Bot, config.Token);
            await client.ConnectAsync();

            var map = new DependencyMap();
            ConfigureServices(map);
        }

        private void ConfigureServices(DependencyMap map)
        {
            map.Add(config);
        }
    }
}