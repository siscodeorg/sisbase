using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace LA_RPbot.Discord.Attributes
{
    public class ImoutoAttribute : CheckBaseAttribute
    {
        public async override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            return ctx.Member.PermissionsIn(ctx.Channel).HasPermission(Permissions.ManageRoles);
        }
    }
}