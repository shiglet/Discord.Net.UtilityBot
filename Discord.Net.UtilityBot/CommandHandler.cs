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
        private readonly Configuration _config;

        private string CommandString => _config.CommandCharacter;
        private bool Mentions => _config.TriggerOnMention;
        private IEnumerable<ulong> Whitelist => _config.ChannelWhitelist;

        public CommandHandler(DependencyMap map)
        {
            _map = map;
            _client = _map.Get<DiscordSocketClient>();
            _client.MessageReceived += HandleCommand;
            _commands = new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false });
        }

        public async Task Configure()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommand(SocketMessage pMsg)
        {
            var message = pMsg as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;
            if (!ParseTriggers(message, ref argPos)) return;

            var context = new SocketCommandContext(_client, message);
            var result = await _commands.ExecuteAsync(context, argPos, _map);
            if (result is SearchResult search)
            {
                await message.AddReactionAsync(UnicodeEmoji.FromText(":mag_right:"));
                await message.AddReactionAsync(UnicodeEmoji.FromText(":question:"));
            }
            else if (result is PreconditionResult precondition)
                await message.AddReactionAsync(UnicodeEmoji.FromText(":no_entry:"));
            else if (result is ParseResult parse)
                await message.Channel.SendMessageAsync($"**Parse Error:** {parse.ErrorReason}");
            else if (result is TypeReaderResult reader)
                await message.Channel.SendMessageAsync($"**Read Error:** {reader.ErrorReason}");
            else if (result is ExecuteResult execute)
            {
                await message.AddReactionAsync(UnicodeEmoji.FromText(":loudspeaker:"));
                await message.Channel.SendMessageAsync($"**Error:** {execute.ErrorReason}");
            }
            else
                await message.AddReactionAsync(UnicodeEmoji.FromText(":rage:"));
        }

        private bool ParseTriggers(SocketUserMessage message, ref int argPos)
        {
            bool flag = (Mentions && message.HasMentionPrefix(_client.CurrentUser, ref argPos)) || (message.HasStringPrefix(CommandString, ref argPos));
            return flag ? Whitelist.Any(id => id == message.Channel.Id) : false;
        }
    }
}
