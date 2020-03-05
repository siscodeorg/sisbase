using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Linq;
using System.Threading.Tasks;

namespace sisbase.Attributes
{
	public class ImoutoAttribute : CheckBaseAttribute
	{
		public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => ctx.Member.Roles.Any(x => x.Permissions.HasPermission(Permissions.ManageRoles));
	}
}