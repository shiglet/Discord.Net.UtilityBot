using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UtilityBot.Services.Configuration;

namespace UtilityBot.Services.GitHub
{
    public class GitHubService
    {
        private readonly Regex IssueRegex = new Regex(@"##([0-9]+)");
        private readonly DiscordSocketClient client;
        private readonly Config config;

        public GitHubService(IDependencyMap map)
        {
            client = map.Get<DiscordSocketClient>();
            config = map.Get<Config>();

            client.MessageReceived += ParseMessage;
        }

        public async Task ParseMessage(SocketMessage message)
        {
            if (!config.ChannelWhitelist.Any(id => message.Channel.Id == id)) return;
            if (message.Author.Id == client.CurrentUser.Id) return;

            MatchCollection matches = IssueRegex.Matches(message.Content);
            if (matches.Count > 0)
            {
                StringBuilder outStr = new StringBuilder();
                foreach (Match match in matches)
                {
                    outStr.AppendLine($"{match.Value} - https://github.com/RogueException/Discord.Net/issues/{match.Value.Substring(2)}");
                }
                await message.Channel.SendMessageAsync(outStr.ToString());
            }
        }
    }
}
