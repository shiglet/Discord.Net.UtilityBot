using Discord.Addons.EmojiTools;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UtilityBot.Services.Configuration;

namespace UtilityBot
{
    public class CommandHandler
    {
        private readonly IDependencyMap _map;
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly Config _config;

        private IEnumerable<ulong> Whitelist => _config.ChannelWhitelist;

        public CommandHandler(DependencyMap map)
        {
            _map = map;
            _client = _map.Get<DiscordSocketClient>();
            _client.MessageReceived += ProcessCommandAsync;
            _commands = _map.Get<CommandService>();
            _config = _map.Get<Config>();
        }

        public async Task ConfigureAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task ProcessCommandAsync(SocketMessage pMsg)
        {
            var message = pMsg as SocketUserMessage;
            if (message == null) return;
            if (message.Content.StartsWith("##")) return;

            int argPos = 0;
            if (!ParseTriggers(message, ref argPos)) return;

            var context = new SocketCommandContext(_client, message);
            var result = await _commands.ExecuteAsync(context, argPos, _map);
            if (result is SearchResult search && !search.IsSuccess)
            {
                await message.AddReactionAsync(UnicodeEmoji.FromText(":mag_right:"));
            }
            else if (result is PreconditionResult precondition && !precondition.IsSuccess)
                await message.AddReactionAsync(UnicodeEmoji.FromText(":no_entry:"));
            else if (result is ParseResult parse && !parse.IsSuccess)
                await message.Channel.SendMessageAsync($"**Parse Error:** {parse.ErrorReason}");
            else if (result is TypeReaderResult reader && !reader.IsSuccess)
                await message.Channel.SendMessageAsync($"**Read Error:** {reader.ErrorReason}");
            else if (result is ExecuteResult execute && !execute.IsSuccess)
            {
                await message.AddReactionAsync(UnicodeEmoji.FromText(":loudspeaker:"));
                await message.Channel.SendMessageAsync($"**Error:** {execute.ErrorReason}");
            }
            else if (!result.IsSuccess)
                await message.AddReactionAsync(UnicodeEmoji.FromText(":rage:"));
        }

        private bool ParseTriggers(SocketUserMessage message, ref int argPos)
        {
            bool flag = false;
            if (message.HasMentionPrefix(_client.CurrentUser, ref argPos)) flag = true;
            else
            {
                foreach (var prefix in _config.CommandStrings)
                {
                    if (message.HasStringPrefix(prefix, ref argPos))
                    {
                        flag = true;
                        break;
                    }
                }
            }
            return flag ? Whitelist.Any(id => id == message.Channel.Id) : false;
        }
    }
}
