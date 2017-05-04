using Discord.Addons.EmojiTools;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UtilityBot.Services.Configuration;
using UtilityBot.Services.Logging;

namespace UtilityBot
{
    public class CommandHandler
    {
        private readonly IServiceProvider _provider;
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly Config _config;
        private readonly ILogger _logger;

        private IEnumerable<ulong> Whitelist => _config.ChannelWhitelist;

        public CommandHandler(IServiceProvider provider)
        {
            _provider = provider;
            _client = _provider.GetService<DiscordSocketClient>();
            _client.MessageReceived += ProcessCommandAsync;
            _commands = _provider.GetService<CommandService>();
            var log = _provider.GetService<LogAdaptor>();
            _commands.Log += log.LogCommand;
            _config = _provider.GetService<Config>();
            _logger = _provider.GetService<Logger>().ForContext<CommandService>();
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
            var result = await _commands.ExecuteAsync(context, argPos, _provider);
            if (result is SearchResult search && !search.IsSuccess)
            {
                await message.AddReactionAsync(EmojiExtensions.FromText(":mag_right:"));
            }
            else if (result is PreconditionResult precondition && !precondition.IsSuccess)
                await message.AddReactionAsync(EmojiExtensions.FromText(":no_entry:"));
            else if (result is ParseResult parse && !parse.IsSuccess)
                await message.Channel.SendMessageAsync($"**Parse Error:** {parse.ErrorReason}");
            else if (result is TypeReaderResult reader && !reader.IsSuccess)
                await message.Channel.SendMessageAsync($"**Read Error:** {reader.ErrorReason}");
            else if (!result.IsSuccess)
                await message.AddReactionAsync(EmojiExtensions.FromText(":rage:"));
            _logger.Debug("Invoked {Command} in {Context} with {Result}", message, context.Channel, result);
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
