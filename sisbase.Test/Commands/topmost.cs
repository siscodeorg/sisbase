using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Test.Commands
{
	public class topmost : BaseCommandModule
	{
		[Command("topmost")]
		public async Task topmostcmd(CommandContext ctx) => await ctx.RespondAsync("this post was made by **topmost**  *gang*");
	}
}
