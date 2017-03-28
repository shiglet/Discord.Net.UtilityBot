﻿using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilityBot.Services.Configuration;

namespace UtilityBot.Preconditions
{
    public class RequireElevatedUserAttribute : PreconditionAttribute
    {
        // TODO: dm-safety
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            var config = map.Get<Config>();

            if (!config.GuildRoleMap.TryGetValue(context.Guild.Id, out IEnumerable<ulong> roles))
                return Task.FromResult(PreconditionResult.FromError("This guild does not have a whitelist."));

            if (!(context.User as SocketGuildUser).Roles.Any(id => roles.Contains(id.Id)))
                return Task.FromResult(PreconditionResult.FromError("You do not have a whitelisted role."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
