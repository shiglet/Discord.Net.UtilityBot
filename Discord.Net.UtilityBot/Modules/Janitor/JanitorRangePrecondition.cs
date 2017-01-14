using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using UtilityBot.Services.Configuration;

namespace UtilityBot.Modules.Janitor
{
    public class RequireJanitorRangeAttribute : ParameterPreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, ParameterInfo parameter, object value, IDependencyMap map)
        {
            int v = (int)value;
            if (v > 10)
            {
                var config = map.Get<Config>();

                if (!config.GuildRoleMap.TryGetValue(context.Guild.Id, out IEnumerable<ulong> roles))
                    return Task.FromResult(PreconditionResult.FromError("This guild does not have a whitelist."));

                if (!(context.User as SocketGuildUser).RoleIds.Any(id => roles.Contains(id)))
                    return Task.FromResult(PreconditionResult.FromError("You do not have a whitelisted role."));

                return Task.FromResult(PreconditionResult.FromSuccess());
            }
            else return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
