using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using sisbase.Attributes;
using sisbase.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Test.Commands
{
	public class attributes : BaseCommandModule
	{
		//With the prefix attribute commands are only executed if run under that specific prefix. 
		//It is CaSe InsEnSitiVE 
		[Prefix("l!")]
		[Command("prefixTest")]
		public async Task prefixTest(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Yay you used the l! prefix"));
	
		//With the imouto attribute commands can only be executed from users that have the modify roles permission.
		[Imouto]
		[Command("imoutoTest")]
		public async Task imoutoTest(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Okaeri Nii~chan!"));
		
		//With the oniisan attribute commands can only be executed on the master server.
		[OniiSan]
		[Command("oniisanTest")]
		public async Task oniisanTest(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("This is the master server."));
	
		/*
		 *  Attributes can be composed toguether 
		 *  
		 *  An command with [Prefix("!"), Imouto, OniiSan] would only be run if all of the below conditions were met:
		 *		
		 *		- The prefix used was !
		 *		- The member had "Modify Roles" permission
		 *		- The server that the command was run were the MASTER server
		 */
	}
}
