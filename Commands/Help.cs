using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using sisbase.Utils;
using System.Threading.Tasks;

namespace sisbase.Commands
{
	public class Help : BaseCommandModule
	{
		// This is a sample help command.
		[Command("help")]
		public async Task helpCommand(CommandContext ctx) => await ctx.RespondAsync(embed: ctx.CommandsNext.HelpEmbed());
	}
}