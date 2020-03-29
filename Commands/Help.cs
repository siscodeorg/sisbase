using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using sisbase.Utils;
using System.Threading.Tasks;

namespace sisbase.Commands
{
	/// <summary>
	/// Base's Help Command
	/// </summary>
	public class Help : BaseCommandModule
	{
		// This is a sample help command.
		[Command("help")]
		public async Task helpCommand(CommandContext ctx) => await ctx.RespondAsync(embed: await ctx.CommandsNext.HelpEmbed(ctx));

		[Command("help")]
		public async Task helpCommand(CommandContext ctx, string options)
		{
			if (options == "-h") await ctx.RespondAsync(embed: await ctx.CommandsNext.HelpEmbed(ctx, true));
		}
	}
}