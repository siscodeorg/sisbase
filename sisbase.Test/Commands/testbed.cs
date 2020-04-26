using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using sisbase.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Test.Commands
{
	[Group("test"), Description("The testbed commands")]
	public class testbed : BaseCommandModule
	{
		[Command("gnmdm")]
		public async Task GetNextMessageDm(CommandContext ctx)
		{
			var dmc = await ctx.Member.CreateDmChannelAsync();
			var msg = await dmc.SendMessageAsync("nmdm : WAITING");
			var response = await dmc.GetNextMessageAsync(ctx.Member);
			await msg.ModifyAsync($"ndmm : \n{response.Result.Content}");
		}

		[Command("mbuild")]
		public async Task MessBuilder(CommandContext ctx)
		{
			var mbuilder = new MessageBuilder();
			mbuilder = mbuilder
				.WithEmbed(EmbedBase.OutputEmbed("Sample Embed"))
				.WithContent("Content");
			await mbuilder.Build(ctx.Channel);
			mbuilder = mbuilder.WithEmbed(mbuilder.Embed.Mutate
				(x =>
					x.WithDescription("Mutated Embed")
						.WithColor(DiscordColor.Red))
				);
			await mbuilder.Build(ctx.Channel);
			mbuilder = mbuilder.WithContent("Mutated Content");
			await mbuilder.Build(ctx.Channel);
		}
	
		[Command("firstInt")]
		public async Task fisrtInt(CommandContext ctx, [RemainingText] string input)
			=> await ctx.RespondAsync(embed: EmbedBase.OutputEmbed($"firstInt : {ctx.Message.FirstInt()}"));

		[Command("firstEmoji")]
		public async Task fisrtEmoji(CommandContext ctx, [RemainingText] string input)
			=> await ctx.RespondAsync(embed: EmbedBase.OutputEmbed($"firstEmoji : {ctx.Message.FirstEmoji() ?? "NO EMOJI FOUND"} `{ctx.Message.FirstEmoji() ?? "NO EMOJI FOUND"}`"));

	}

	[Group("stubgroup")]
	public class stub : BaseCommandModule
	{
		[GroupCommand()]
		public async Task stubCmd(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.GroupHelpEmbed(ctx.Command));
	}
}