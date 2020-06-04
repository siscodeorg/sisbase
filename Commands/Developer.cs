using DSharpPlus.CommandsNext;
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

namespace sisbase.Commands
{
#pragma warning disable CS1591

	/// <summary>
	/// Bot-Owner Only Commands
	/// </summary>
	public class Developer : BaseCommandModule
	{
		[Command("setMaster")]
		[RequireOwner,RequireSystem(typeof(MasterServer))]
		public async Task SetMaster(CommandContext ctx)
		{
			var guilds = SisbaseBot.Instance.Client.Guilds.Values.ToList();
			var ids = new List<ulong?>();
			var embed = new DiscordEmbedBuilder();
			embed
				.WithAuthor($"Set {ctx.Guild.Name} as MASTER")
				.WithColor(DiscordColor.PhthaloGreen);

			guilds.Remove(ctx.Guild);
			SisbaseBot.Instance.SisbaseConfiguration.Config.MasterId = ctx.Guild.Id;
			guilds.ForEach(x => ids.Add(x.Id));
			SisbaseBot.Instance.SisbaseConfiguration.Config.PuppetId = ids;
			File.WriteAllText(Directory.GetCurrentDirectory() + "/Config.json",
				JsonConvert.SerializeObject(SisbaseBot.Instance.SisbaseConfiguration.Config, Formatting.Indented));
			await ctx.RespondAsync(embed: embed);
		}
	}

	[OniiSan]
	[Imouto]
	[Emoji(":computer:")]
	[Group("system")]
	[Description("This group configures the systems.")]
	public class System : BaseCommandModule
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
			var allSystems = SMC.RegisteredSystems.ToList().Select(x => $"{(x.Value.IsVital() ? "\\⚠️" : "")} {x.Value.Name} - `{x.Key.Assembly.GetName().Name}`");
			var embed = EmbedBase.ListEmbed(allSystems, "Systems");
			embed = embed.Mutate(x => x
				.AddField("Permanently disabled systems [Systems.json]",
					string.Join("\n",
						SisbaseBot.Instance.SystemCfg.Systems.Where(kvp => !kvp.Value.Enabled)
							.Select(kvp => kvp.Key))));
			await ctx.RespondAsync(embed: embed.Mutate(x => x.WithFooter($"{x.Footer.Text}  | ⚠️ - Vital")));
		}

		[Command("disable")]
		[Description("Disables and unregisters a system")]
		public async Task DisableFail(CommandContext ctx) =>
			await ctx.RespondAsync(embed: EmbedBase.CommandHelpEmbed(ctx.Command));
		[Command("disable")]
		public async Task Disable(CommandContext ctx, [DSharpPlus.CommandsNext.Attributes.Description("If the system is to be disabled permanently `true`|`false`")] bool permanent)
		{
			var allSystems = SMC.RegisteredSystems.Where(s => !s.Value.IsVital()).Select(k => k.Value.Name).ToList();
			var embed = EmbedBase.OrderedListEmbed(allSystems, "Systems").Mutate(x =>
			x.WithTitle("Please select the system you want to disable [number]")
			 .WithAuthor(null)
			 .WithColor(DiscordColor.Red));
			var message = await ctx.RespondAsync(embed: embed);
			var response = await ctx.Message.GetNextMessageAsync();
			if(response.TimedOut) return;
			var select = response.Result.FirstInt();
			var systemType = SMC.RegisteredSystems.Where(x => x.Value.Name == allSystems[select]).FirstOrDefault();
			SMC.Unregister(systemType.Key);
			if (permanent) {
				SisbaseBot.Instance.SystemCfg.Systems[systemType.Key.ToCustomName()].Enabled = false;
				SisbaseBot.Instance.SystemCfg.Update();
			}
			await message.ModifyAsync(embed: EmbedBase.OutputEmbed($"Successfully unregistered {allSystems[select]} {(permanent?"permanently":"temporarily")}."));
		}
		[Command("reload")]
		[Description("Reloads the SMC and registers any Systems that weren't registered")]
		public async Task Reload(CommandContext ctx)
		{
			var Data = SisbaseBot.Instance.Systems.Reload();
			if (Data.Any(k => k.Value.Count > 0))
			{
				var Embed = EmbedBase.OutputEmbed("SMC Reloaded. ΔSystem Status:");

				foreach (var kvp in Data)
				{
					var i1 = kvp.Value.Select(x => $"{(x.Value ? "✅" : "❌")} - `{x.Key}`").ToList();
					string i0 = string.Join("\n", i1);
					string i2 = kvp.Key.GetName().Name;
					if (string.IsNullOrEmpty(i0)) continue;
					Embed = Embed.Mutate(x => x.AddField(i2, i0));
				}
				await ctx.RespondAsync(embed: Embed);
			}
			else
			{
				await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("All Systems were already loaded. No new systems were registerd"));
			}
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
					if (SisbaseBot.Instance.SisbaseConfiguration.Config.Prefixes.Contains(prefix.ToLowerInvariant()))
					{
						await msg.ModifyAsync(embed: EmbedBase.OutputEmbed($"This prefix is already added."));
					}
					else
					{
						SisbaseBot.Instance.SisbaseConfiguration.Config.Prefixes.Add(prefix.ToLowerInvariant());
						File.WriteAllText(SisbaseBot.Instance.SisbaseConfiguration.JsonPath,
							JsonConvert.SerializeObject(SisbaseBot.Instance.SisbaseConfiguration.Config, Formatting.Indented));
						await msg.ModifyAsync(embed: EmbedBase.OutputEmbed($"Prefix added without errors."));
					}

					break;

				case "del":
					var msg2 = await ctx.RespondAsync(embed: EmbedBase.InputEmbed("Prefix to be removed"));
					var response2 = await ctx.Message.GetNextMessageAsync();
					string prefix2 = response2.Result.Content;
					if (!SisbaseBot.Instance.SisbaseConfiguration.Config.Prefixes.Contains(prefix2.ToLowerInvariant()))
					{
						await msg2.ModifyAsync(embed: EmbedBase.OutputEmbed($"This prefix doesn't exists."));
					}
					else
					{
						SisbaseBot.Instance.SisbaseConfiguration.Config.Prefixes.Remove(prefix2.ToLowerInvariant());
						File.WriteAllText(SisbaseBot.Instance.SisbaseConfiguration.JsonPath,
							JsonConvert.SerializeObject(SisbaseBot.Instance.SisbaseConfiguration.Config, Formatting.Indented));
						await msg2.ModifyAsync(embed: EmbedBase.OutputEmbed($"Prefix removed without errors."));
					}
					break;

				case "list":
					await ctx.RespondAsync(embed: EmbedBase.OrderedListEmbed(SisbaseBot.Instance.SisbaseConfiguration.Config.Prefixes, "Prefixes"));
					break;

				default:
					var embed = EmbedBase.CommandHelpEmbed(ctx.Command);
					await ctx.RespondAsync(embed: embed);
					break;
			}
		}
	}
}