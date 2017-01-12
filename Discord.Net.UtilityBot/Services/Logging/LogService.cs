using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UtilityBot.Services.Logging
{
    // todo: replace this with a real logging implementation
    public class LogService
    {
        public LogService(IDependencyMap map)
        {
            var client = map.Get<DiscordSocketClient>();
            client.Log += Log;
        }

        public Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }
    }
}
