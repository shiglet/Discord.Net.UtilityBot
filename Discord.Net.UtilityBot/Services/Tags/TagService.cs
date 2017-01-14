using Discord;
using Discord.Commands;
using Discord.Commands.Builders;
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

        private readonly CommandService commands;

        private ModuleInfo tagModule;

        public TagService(IDependencyMap map)
        {
            commands = map.Get<CommandService>();
            db = TagDb.Load();
        }

        public async Task BuildCommands()
        {
            if (tagModule != null)
                await commands.RemoveModuleAsync(tagModule);

            tagModule = await commands.CreateModuleAsync("", module =>
            {
                module.Name = "Tags";
                
                foreach (var tag in db.Tags)
                {
                    module.AddCommand(tag.Name, async (context, args, map) =>
                    {
                        var builder = new EmbedBuilder()
                            .WithTitle(tag.Name)
                            .WithDescription(tag.Content);
                        var user = await context.Channel.GetUserAsync(tag.OwnerId);
                        if (user != null)
                            builder.Author = new EmbedAuthorBuilder()
                                .WithIconUrl(user.AvatarUrl)
                                .WithName(user.Username);

                        await context.Channel.SendMessageAsync("", embed: builder.Build());
                    }, builder =>
                    {
                        builder.AddAliases(tag.Aliases.ToArray());
                    });
                }
            });
        }

        public async Task AddTag(Tag tag)
        {
            db.Tags.Add(tag);
            db.Save();
            await BuildCommands();
        }
        public async Task RemoveTag(Tag tag)
        {
            db.Tags.Remove(tag);
            db.Save();
            await BuildCommands();
        }
    }
}
