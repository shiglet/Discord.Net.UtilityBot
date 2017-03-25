using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.WebSocket;
using UtilityBot.Services.Configuration;

namespace UtilityBot.Services.GitHub
{
    public class GitHubService
    {
        private readonly Regex _issueRegex = new Regex(@"##([0-9]+)");
        private readonly DiscordSocketClient _client;
        private readonly Config _config;

        public GitHubService(DiscordSocketClient client, Config config)
        {
            client.MessageReceived += ParseMessage;

            _client = client;
            _config = config;
        }

        public async Task ParseMessage(SocketMessage message)
        {
            if (_config.ChannelWhitelist.All(id => message.Channel.Id != id)) return;
            if (message.Author.Id == _client.CurrentUser.Id) return;

            var matches = _issueRegex.Matches(message.Content);
            if (matches.Count > 0)
            {
                var outStr = new StringBuilder();
                foreach (Match match in matches)
                {
                    outStr.AppendLine($"{match.Value} - https://github.com/RogueException/Discord.Net/issues/{match.Value.Substring(2)}");
                }
                await message.Channel.SendMessageAsync(outStr.ToString());
            }
        }
    }
}
