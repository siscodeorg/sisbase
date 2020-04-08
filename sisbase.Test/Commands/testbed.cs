using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
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
	}
}