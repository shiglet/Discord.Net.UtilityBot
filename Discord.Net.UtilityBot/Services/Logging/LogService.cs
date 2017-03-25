using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace UtilityBot.Services.Logging
{
    // todo: replace this with a real logging implementation
    public class LogService
    {
        public LogService(DiscordSocketClient client)
        {
            client.Log += Log;
        }

        public Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        public async Task LogCommand(LogMessage message)
        {
            if (message.Exception != null && message.Exception is CommandException cmd)
            {
                await cmd.Context.Channel.SendMessageAsync(cmd.ToString());
            }
            await Log(message);
        }
    }
}
