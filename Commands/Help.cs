using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using sisbase.Builders;

namespace sisbase.Commands
{
	/// <summary>
	/// Base's Help Command
	/// </summary>
	public class Help : BaseCommandModule
	{
		// This is a sample help command.
		[Command("help")]
#pragma warning disable CS1591
		public async Task helpCommand(CommandContext ctx) => await ctx.RespondAsync(embed: await ctx.CommandsNext.HelpEmbed(ctx));

		[Command("help")]
		public async Task helpCommand(CommandContext ctx, string options)
		{
			if (options == "-h") await ctx.RespondAsync(embed: await ctx.CommandsNext.HelpEmbed(ctx, true));
		}
	}
}