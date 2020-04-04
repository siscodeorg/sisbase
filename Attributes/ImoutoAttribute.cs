using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Linq;
using System.Threading.Tasks;

namespace sisbase.Attributes
{
	/// <summary>
	/// Attrribute that checks if the user is a staff member (Has Modify Roles permission)
	/// </summary>
	public class ImoutoAttribute : CheckBaseAttribute
	{
#pragma warning disable CS1998, CS1591

		public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => ctx.Member.Roles.Any(x => x.Permissions.HasPermission(Permissions.ManageRoles));

#pragma warning restore CS1998
	}
}