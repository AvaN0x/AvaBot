using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS1998
namespace AvaBot
{
    public class RequireAdminRoleAttribute : PreconditionAttribute
    {

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context,
            CommandInfo command, IServiceProvider services)
        {
            ulong? _roleId = Utils.GetSettings(context.Guild.Id).adminRoleId;

            if (_roleId == null)
                return PreconditionResult.FromError("There is not setted role for this guild.");

            var guildUser = context.User as IGuildUser;
            if (guildUser == null)
                return PreconditionResult.FromError("This command cannot be executed outside of a guild.");

            var guild = guildUser.Guild;
            if (guild.Roles.All(r => r.Id != _roleId))
                return PreconditionResult.FromError(
                    $"The guild does not have the role ({_roleId}) required to access this command.");

            return guildUser.RoleIds.Any(rId => rId == _roleId)
                ? PreconditionResult.FromSuccess()
                : PreconditionResult.FromError("You do not have the sufficient role required to access this command.");
        }
    }

    public class RequireSettingAttribute : PreconditionAttribute
    {
        private readonly string _name;

        public RequireSettingAttribute(string name) => _name = name;

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context,
            CommandInfo command, IServiceProvider services)
        {
            var guildUser = context.User as IGuildUser;
            if (guildUser == null)
                return PreconditionResult.FromError("This command cannot be executed outside of a guild.");

            var settings = Utils.GetSettings(context.Guild.Id);
            var type = settings.GetType();
            var props = type.GetProperties();
            var focused = props.FirstOrDefault(prop => prop.Name == _name);

            if (focused == null)
                return PreconditionResult.FromError("The setting required (" + _name + ") cannot be found.");

            return (bool)focused.GetValue(settings)
                ? PreconditionResult.FromSuccess()
                : PreconditionResult.FromError("This guild have disabled this command.");
        }
    }

}
