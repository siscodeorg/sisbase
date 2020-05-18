using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using sisbase.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using static sisbase.Utils.Behaviours;

namespace sisbase.Test.Commands {
	/// <summary>
	/// Examples on how to use the new behaviours
	/// </summary>
	public class Behaviours : BaseCommandModule {
		[Command("counting")]
		public async Task CountingCommandFailed(CommandContext ctx) =>
			await ctx.RespondAsync(embed: EmbedBase.CommandHelpEmbed(ctx.Command));

		[Command("counting")]
		public async Task CountingCommand(CommandContext ctx, [Description("Ordinal Flag")] bool useOrdinal = false) {
			var list = new List<string> { "One", "Two", "Three" };
			await ctx.RespondAsync(embed: EmbedBase.OrderedListEmbed(list,
				$"Numbers {(useOrdinal ? "with" : "without")} ordinal",
				useOrdinal ? CountingBehaviour.Ordinal : CountingBehaviour.Default));
		}
	}
}