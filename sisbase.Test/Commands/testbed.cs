using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using sisbase.Interactivity;
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

		[Command("interact")]
		public async Task interactCmd(CommandContext ctx)
		{
			var interactResult = await ctx.Member.InteractAsync(x => x.WithContent("Please Type Something"), ctx.Channel);
			await interactResult.RespondAsync($"YOOOO > {interactResult.Content}");
			var message2 = await ctx.Member.InteractAsync(x => x.WithEmbed(EmbedBase.OutputEmbed("Test2, Say something Again")), ctx.Channel);
			await message2.RespondAsync(embed: EmbedBase.OutputEmbed($"YOOOO > {message2.Content}"));
		}
	}

	[Group("stubgroup")]
	public class stub : BaseCommandModule
	{
		[GroupCommand()]
		public async Task stubCmd(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.GroupHelpEmbed(ctx.Command));
	}
}