using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilityBot.Services.Configuration;

namespace UtilityBot.Preconditions
{
    public class RequireElevatedUserAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            if (context.Guild == null)
                return Task.FromResult(PreconditionResult.FromError("This command may only be run in a guild."));

            var config = map.Get<Config>();

            if (!config.GuildRoleMap.TryGetValue(context.Guild.Id, out IEnumerable<ulong> roles))
                return Task.FromResult(PreconditionResult.FromError("This guild does not have a whitelist."));

            if (!(context.User as SocketGuildUser).Roles.Any(id => roles.Contains(id.Id)))
                return Task.FromResult(PreconditionResult.FromError("You do not have a whitelisted role."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
