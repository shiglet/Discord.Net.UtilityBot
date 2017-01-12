using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UtilityBot.Services.Configuration;

namespace UtilityBot.Services.Tags
{
    public class TagService
    {
        public readonly TagDb db;

        private readonly DiscordSocketClient client;
        private readonly Configuration.Configuration config;

        public TagService(IDependencyMap map)
        {
            client = map.Get<DiscordSocketClient>();
            db = TagDb.Load();
            db.RebuildMap();
            config = map.Get<Configuration.Configuration>();

            client.MessageReceived += MessageReceived;
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if (message.Author == client.CurrentUser) return;
            foreach (var prefix in config.TagStrings)
            {
                if (!message.Content.StartsWith(prefix)) continue;
                var tagPath = message.Content.Substring(prefix.Length);

                if (!db.TagMap.TryGetValue(tagPath, out Tag tag)) return;

                var builder = new EmbedBuilder()
                    .WithTitle(tag.Name)
                    .WithDescription(tag.Content);
                var user = await message.Channel.GetUserAsync(tag.OwnerId);
                if (user != null)
                    builder.Author = new EmbedAuthorBuilder()
                        .WithIconUrl(user.AvatarUrl)
                        .WithName(user.Username);

                await message.Channel.SendMessageAsync("", embed: builder.Build());
                return;
            }
        }
    }
}
