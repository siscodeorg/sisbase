using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace sisbase.Attributes
{
	public class OniiSanAttribute : CheckBaseAttribute
	{
#pragma warning disable CS1998
		public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => ctx.Guild.Id.Equals(SisbaseBot.Instance.SisbaseConfiguration.Config.MasterId);
#pragma warning restore CS1998
	}
}