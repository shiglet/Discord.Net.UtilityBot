using Discord;
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
    }
}
