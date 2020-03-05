using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace sisbase.Attributes
{
	public class OniiSanAttribute : CheckBaseAttribute
	{
		public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => ctx.Guild.Id.Equals(SisbaseBot.Instance.SisbaseConfiguration.Config.MasterId);
	}
}