﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using sisbase.Attributes;
using sisbase.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using sisbase.Systems;
using DSharpPlus.Interactivity.Extensions;
using sisbase.CommandsNext;
using System;

namespace sisbase.Commands
{
#pragma warning disable CS1591

	/// <summary>
	/// Bot-Owner Only Commands
	/// </summary>
	public class Developer : BaseCommandModule
	{
		[Command("setMaster")]
		[RequireOwner]
		public async Task SetMaster(CommandContext ctx)
		{
			var guilds = SisbaseBot.Instance.Client.Guilds.Values.ToList();
			var ids = new List<ulong>();
			var embed = new DiscordEmbedBuilder();
			embed
				.WithAuthor($"Set {ctx.Guild.Name} as MASTER")
				.WithColor(DiscordColor.PhthaloGreen);

			guilds.Remove(ctx.Guild);
			SisbaseBot.Instance.SisbaseConfiguration.Data.MasterId = ctx.Guild.Id;
			guilds.ForEach(x => ids.Add(x.Id));
			SisbaseBot.Instance.SisbaseConfiguration.Data.PuppetId = ids;
			SisbaseBot.Instance.SisbaseConfiguration.Update();
			await ctx.RespondAsync(embed: embed);
		}
	}

	[OniiSan]
	[Imouto]
	[Emoji(":computer:")]
	[Group("system")]
	[Description("This group configures the systems.")]
	public class System : SisbaseCommandModule
	{
		[GroupCommand]
		public async Task Command(CommandContext ctx)
		{
			var embed = EmbedBase.GroupHelpEmbed(ctx.Command);
			await ctx.RespondAsync(embed: embed);
		}

		[Command("list")]
		[Description("Lists all active systems")]
		public async Task List(CommandContext ctx)
		{
			var loadedSystems = SisbaseInstance.SystemManager.Systems;
			var unloadedSystems = SisbaseInstance.SystemManager.UnloadedSystems;
			var disabledSystems = SisbaseInstance.SystemCfg.Systems.Where(x => x.Value.Enabled == false);
			string loadedStr = string.Join("\n", loadedSystems.Select(x => $"{(x.Value.IsVital() ? "⛔" : "")} [{x.Key.Assembly.GetName().Name}] {x.Value.Name} {(string.IsNullOrWhiteSpace(x.Value.Description) ? "" : $"<{x.Value.Description}>")}"));
			string unloadedStr = string.Join("\n", unloadedSystems.Select(x => $"{x.Key.ToCustomName()}"));
			string disabledStr = string.Join("\n", disabledSystems.Select(x => x.Key));
			var embed = EmbedBase.OutputEmbed("")
				.Mutate(x => x
					.WithAuthor("Systems - List")
					.AddField("Loaded Systems",$"{(string.IsNullOrEmpty(loadedStr) ? "No systems loaded." : loadedStr)}")
					.AddField("Unloaded Systems", $"{(string.IsNullOrEmpty(unloadedStr) ? "No systems unloaded." : unloadedStr)}")
					.AddField("Permanently Disabled Systems", $"{(string.IsNullOrEmpty(disabledStr) ? "No systems disabled." : disabledStr)}")
					.WithFooter($"{x.Footer.Text} | ⛔ - Vital System",x.Footer.IconUrl)
				);
			await ctx.RespondAsync(embed: embed);
		}

		[Command("unload")]
		[Description("Unloads a system")]
		public async Task Unload(CommandContext ctx) {
			var loadedSystems = SisbaseInstance.SystemManager.Systems;
			var embed = EmbedBase.OrderedListEmbed(loadedSystems.Select(x => x.Value.Name).ToList(), "Systems")
				.Mutate(x => x.WithAuthor("Please select the system to be unloaded [Number]"));
			var msg = await ctx.RespondAsync(embed: embed);
			var response = await ctx.Message.GetNextMessageAsync();
			if (response.TimedOut) {
				await msg.ModifyAsync(embed: EmbedBase.OutputEmbed("Timed Out."));
				return;
			}
			var num = Math.Clamp(response.Result.FirstInt(), 0, loadedSystems.Count - 1);
			var system = loadedSystems.Keys.ToList()[num];
			var result = await SisbaseInstance.SystemManager.TryUnregisterType(system);
            if (result) {
				await msg.ModifyAsync(embed: EmbedBase.OutputEmbed($"{system.ToCustomName()} unloaded successfully!"));
            } else {
				await msg.ModifyAsync(embed: EmbedBase.OutputEmbed($"{system.ToCustomName()} could not be unloaded."));
			}
		}
		[Command("reload")]
		[Description("Reloads the SMC and registers any Systems that weren't registered")]
		public async Task Reload(CommandContext ctx)
		{
			await SisbaseInstance.SystemManager.ReloadTempUnloadedSystems();
			await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Reload completed."));
		}
	}

	[OniiSan] // Sets group to be only executable on the master server
	[Imouto] // Sets group to be only executable by the staff (Modify Roles)
	[Emoji(":wrench:")] // Sets the emoji for the group
	[Group("config")]
	[Description("This group configures the bot.")]
	public class Config : BaseCommandModule
	{
		[GroupCommand]
		public async Task Command(CommandContext ctx)
		{
			var embed = EmbedBase.GroupHelpEmbed(ctx.Command);
			await ctx.RespondAsync(embed: embed);
		}

		[Command("prefix")]
		[Description("Changes the custom prefixes")]
		public async Task PrefixError(CommandContext ctx)
		{
			var embed = EmbedBase.CommandHelpEmbed(ctx.Command);
			await ctx.RespondAsync(embed: embed);
		}

		// Sample of a command that uses interactivity and sisbase to input/output text data.
		[Command("prefix")]
		public async Task PrefixSuccess(CommandContext ctx, [DSharpPlus.CommandsNext.Attributes.Description("The operation to be executed [add/list/del] ")]
			string operation)
		{
			switch (operation.ToLowerInvariant())
			{
				case "add":
					var msg = await ctx.RespondAsync(embed: EmbedBase.InputEmbed("Prefix to be added"));
					var response = await ctx.Message.GetNextMessageAsync();
					string prefix = response.Result.Content;
					if (SisbaseBot.Instance.SisbaseConfiguration.Data.Prefixes.Contains(prefix.ToLowerInvariant()))
					{
						await msg.ModifyAsync(embed: EmbedBase.OutputEmbed($"This prefix is already added."));
					}
					else
					{
						SisbaseBot.Instance.SisbaseConfiguration.Data.Prefixes.Add(prefix.ToLowerInvariant());
						SisbaseBot.Instance.SisbaseConfiguration.Update();
						await msg.ModifyAsync(embed: EmbedBase.OutputEmbed($"Prefix added without errors."));
					}

					break;

				case "del":
					var msg2 = await ctx.RespondAsync(embed: EmbedBase.InputEmbed("Prefix to be removed"));
					var response2 = await ctx.Message.GetNextMessageAsync();
					string prefix2 = response2.Result.Content;
					if (!SisbaseBot.Instance.SisbaseConfiguration.Data.Prefixes.Contains(prefix2.ToLowerInvariant()))
					{
						await msg2.ModifyAsync(embed: EmbedBase.OutputEmbed($"This prefix doesn't exists."));
					}
					else
					{
						SisbaseBot.Instance.SisbaseConfiguration.Data.Prefixes.Remove(prefix2.ToLowerInvariant());
						SisbaseBot.Instance.SisbaseConfiguration.Update();
						await msg2.ModifyAsync(embed: EmbedBase.OutputEmbed($"Prefix removed without errors."));
					}
					break;

				case "list":
					await ctx.RespondAsync(embed: EmbedBase.OrderedListEmbed(SisbaseBot.Instance.SisbaseConfiguration.Data.Prefixes, "Prefixes"));
					break;

				default:
					var embed = EmbedBase.CommandHelpEmbed(ctx.Command);
					await ctx.RespondAsync(embed: embed);
					break;
			}
		}
	}
}
