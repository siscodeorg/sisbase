using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using sisbase.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Test.Commands
{
	/// <summary>
	/// Hidden commands are hidden from the help command (unless if -h is added to the command)
	/// </summary>
	public class hidden : BaseCommandModule
	{
		[Command("hidden")]
		//To make a command hidden you only need to add this attribute.
		[Hidden]
		public async Task hiddenCommand(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("I'm a sneaky snek."));
	}
}
