using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityBot.Modules.Janitor
{
    public class JanitorModule : ModuleBase<SocketCommandContext>
    {
        [Command("tag clean")]
        [Alias("clean", "clear", "cleanup", "c")]
        [Priority(1000)]
        public async Task Clean(int count = 15)
        {
            int index = 0;
            List<IMessage> delete = new List<IMessage>(count);
            await Context.Channel.GetMessagesAsync().ForEachAsync(async batch =>
            {
                var messages = batch.Where(m => m.Author.Id == Context.Client.CurrentUser.Id);

                foreach (var msg in messages.OrderByDescending(msg => msg.Timestamp))
                {
                    if (index >= count)
                    {
                        await EndClean(delete);
                        return;
                    }
                    delete.Add(msg);
                    index++;
                }
            });
        }

        private async Task EndClean(IEnumerable<IMessage> messages)
        {
            foreach (var message in messages)
            {
                await message.DeleteAsync();
            }
        }
    }
}
