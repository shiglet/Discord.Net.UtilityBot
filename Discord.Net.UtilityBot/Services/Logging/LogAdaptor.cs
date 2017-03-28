using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace UtilityBot.Services.Logging
{
    public class LogAdaptor
    {
        private readonly Logger _logger;

        public static Logger CreateLogger()
        {
           return new LoggerConfiguration()
                .WriteTo.ColoredConsole()
#if RELEASE
                .WriteTo.RollingFile("log-{Date}.log")
#endif
                .CreateLogger();
        }

        public LogAdaptor(Logger logger, DiscordSocketClient client)
        {
            _logger = logger;
            client.Log += LogAsync;
        }

        private Task LogAsync(LogMessage message)
        {
            _logger.Write(
                GetEventLevel(message.Severity),
                "{source} {message}",
                message.Source,
                message.Message,
                message.Exception);

            return Task.CompletedTask;
        }

        internal async Task LogCommand(LogMessage message)
        {
            if (message.Exception != null && message.Exception is CommandException cmd)
            {
                await cmd.Context.Channel.SendMessageAsync(cmd.ToString());
            }
            await LogAsync(message);
        }

        private static LogEventLevel GetEventLevel(LogSeverity severity)
        {
            return (LogEventLevel) Math.Abs((int) (severity - 5));
        }
    }
}
