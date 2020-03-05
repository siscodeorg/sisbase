using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Linq;
using System.Threading.Tasks;

namespace sisbase.Attributes
{
	public class ImoutoAttribute : CheckBaseAttribute
	{
#pragma warning disable CS1998
		public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => ctx.Member.Roles.Any(x => x.Permissions.HasPermission(Permissions.ManageRoles));
#pragma warning restore CS1998
	}
}