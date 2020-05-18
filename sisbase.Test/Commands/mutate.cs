using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using sisbase.Utils;
using System.Threading.Tasks;

namespace sisbase.Test {
	//Example on how to use the new mutate function.
	//You can use it on every DiscordEmbed. We only used on a EmbedBase' embed for simplicity sake.
	public class Mutate : BaseCommandModule {
		[Command("mutate")]
		public async Task MutateCmd(CommandContext ctx) {
			var embed = EmbedBase.OutputEmbed("Starting Embed");
			await ctx.RespondAsync(embed: embed);
			await ctx.RespondAsync(embed: embed.Mutate(x => x.WithTitle("Mutated Embed")));
			await ctx.RespondAsync(embed: embed
					.Mutate(x => {
						x
						.WithTitle("Fancy - Mutated Embed")
						.WithColor(DiscordColor.Red)
						.AddField("Yeah", "That was fancy!");
					}));
		}
	}
}