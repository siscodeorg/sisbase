using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using sisbase.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static sisbase.Utils.Behaviours;

namespace sisbase.Utils {
    /// <summary>
    /// Utility for generating consistant embeds
    /// </summary>
    public static class EmbedBase
	{
		/// <summary>
		/// Generates a new group help embed from an specified command <br></br> The comand must be
		/// a GroupCommand.
		/// </summary>
		/// <param name="Command"></param>
		/// <returns></returns>
		public static DiscordEmbed GroupHelpEmbed(CommandGroup group)
		{
			var formattedChilren = group.Children.Distinct().Select(x => $"{x.Name} - {x.Description}");
			var groupHelpEmbed = new DiscordEmbedBuilder();
			groupHelpEmbed
				.WithFooter($"「sisbase」・ {General.GetVersion()}", "https://i.imgur.com/6ovRzR9.png")
				.WithDescription(group.Description)
				.WithAuthor($"Group : {group.Name} | Help")
				.WithColor(DiscordColor.Gray);
            if (formattedChilren.Any()) {
				groupHelpEmbed.AddField("Sub-commands", string.Join("\n", formattedChilren));
            }
			return groupHelpEmbed.Build();
		}

		/// <summary>
		/// Generates the bot help command embed.
		/// </summary>
		/// <param name="cne"></param>
		/// <param name="ctx"></param>
		/// <param name="showHidden"></param>
		/// <returns></returns>
		public static async Task<DiscordEmbed> HelpEmbed(this CommandsNextExtension cne, CommandContext ctx, bool showHidden = false)
		{
			var helpBuilder = new DiscordEmbedBuilder();
			var unk = DiscordEmoji.FromName(SisbaseBot.Instance.Client, ":grey_question:");
			foreach (var group in await cne.GetAllowedGroupsAsync(ctx, showHidden))
			{
				helpBuilder.AddField($"{group.GetGroupEmoji(unk)} ・ {group.Name}", group.GetGroupDescription());
			}
			var topLevel = await cne.GetAllowedTopLevelCommandsAsync(ctx, showHidden);
			var remainingCommands = topLevel.Select(x => $"`{x.Name}`");
			if(remainingCommands.Any()) helpBuilder.AddField("❓ ・ Miscellaneous ", string.Join(" ", remainingCommands));
			helpBuilder
				.WithDescription($"To see help for a group run {SisbaseBot.Instance.Client.CurrentUser.Mention} `group name`")
				.WithFooter($"「sisbase」・ {General.GetVersion()}", "https://i.imgur.com/6ovRzR9.png")
				.WithAuthor("Help | Showing all groups")
				.WithColor(DiscordColor.CornflowerBlue);
			return helpBuilder.Build();
		}

		internal static async Task<List<Command>> GetAllowedTopLevelCommandsAsync(this CommandsNextExtension cne, CommandContext ctx, bool hidden) {
			var commands = cne.RegisteredCommands.Values.ToList().Distinct();
			var allowedCommands = new List<Command>();
			foreach (var command in commands.Distinct()) {
				if (await command.CheckAsync(ctx, hidden))
					allowedCommands.Add(command);
			}
			return allowedCommands.Where(x => !(x is CommandGroup)).ToList();
		}

		internal static async Task<List<CommandGroup>> GetAllowedGroupsAsync(this CommandsNextExtension cne, CommandContext ctx, bool hidden) {
			var groups = cne.RegisteredCommands.Values.ToList().OfType<CommandGroup>().Distinct();
			var allowedGroups = new List<CommandGroup>();
			foreach (var group in groups) {
				if (await group.CheckAsync(ctx, hidden))
					allowedGroups.Add(group);
			}
			return allowedGroups;
		}

		internal static string GetGroupDescription(this CommandGroup group) {
            if (string.IsNullOrWhiteSpace(group.Description)) {
				return $"{group.Name} - STUB : Doesn't have a description.";
            }
			return group.Description;
        }

		internal static DiscordEmoji GetGroupEmoji(this CommandGroup group, DiscordEmoji @default) {
			var query = group.CustomAttributes.FirstOrDefault(x => x is EmojiAttribute);
			if (query == default) {
				return @default;
            }
			return ((EmojiAttribute)query).Emoji;
        }

		internal static async Task<bool> CheckAsync(this Command command, CommandContext ctx, bool hidden) {
			if ((await command.RunChecksAsync(ctx, true)).Count() > 0) return false;
			if (command.IsHidden && !hidden) return false;
			return true;
		}

		/// <summary>
		/// Generates a new <see cref="DiscordEmbed"/> from a given query string. <br></br> Prepends
		/// "Please Type : " to the given string.
		/// </summary>
		/// <param name="input">The string</param>
		/// <returns></returns>
		public static DiscordEmbed InputEmbed(string input)
		{
			var inputEmbedBuilder = new DiscordEmbedBuilder();
			inputEmbedBuilder
				.WithFooter($"「sisbase」・ {General.GetVersion()}", "https://i.imgur.com/6ovRzR9.png")
				.WithDescription($"Please type : {input}")
				.WithColor(DiscordColor.MidnightBlue);
			return inputEmbedBuilder.Build();
		}

		/// <summary>
		/// Generates a new <see cref="DiscordEmbed"/> from a given string
		/// </summary>
		/// <param name="output">The string that will be set as the description</param>
		/// <returns></returns>
		public static DiscordEmbed OutputEmbed(string output)
		{
			var outputEmbedBuilder = new DiscordEmbedBuilder();
			outputEmbedBuilder
				.WithFooter($"「sisbase」・ {General.GetVersion()}", "https://i.imgur.com/6ovRzR9.png")
				.WithDescription($"{output}")
				.WithColor(DiscordColor.SpringGreen);
			return outputEmbedBuilder.Build();
		}

		/// <summary>
		/// Generates a new <see cref="DiscordEmbed"/> with all the items on that list <br></br>
		/// with an index added to each entry.
		/// </summary>
		/// <typeparam name="T">The type of said list</typeparam>
		/// <param name="list">The list</param>
		/// <param name="name">Name that will be displayed on the embed.</param>
		/// <param name="behaviour">An <see cref="CountingBehaviour"/>, defaults to <see cref="CountingBehaviour.Default"/></param>
		/// <returns></returns>
		public static DiscordEmbed OrderedListEmbed<T>(List<T> list, string name,
			CountingBehaviour behaviour = CountingBehaviour.Default)
		{
			var listData = list.Select(x => $"{GetNumber(list.IndexOf(x),behaviour)}・{x}");
			string data = string.Join("\n",listData);
			var orderedListBuilder = new DiscordEmbedBuilder();
			orderedListBuilder
				.WithAuthor($"List of : {name}")
				.WithFooter($"「sisbase」・ {General.GetVersion()}", "https://i.imgur.com/6ovRzR9.png")
				.WithDescription(string.IsNullOrWhiteSpace(data) ? "No data" : data)
				.WithColor(DiscordColor.Orange);
			return orderedListBuilder.Build();
		}

        internal static int GetNumber(int num, CountingBehaviour behaviour) => behaviour switch
        {
            CountingBehaviour.Ordinal => num + 1,
            CountingBehaviour.Default => num,
            _ => num
        };

        /// <summary>
        /// Generates a new <see cref="DiscordEmbed"/> with all the items on that list
        /// </summary>
        /// <typeparam name="T">The type of said list</typeparam>
        /// <param name="list">The list</param>
        /// <param name="name">Name that will be displayed on the embed.</param>
        /// <returns></returns>
        public static DiscordEmbed ListEmbed<T>(List<T> list, string name)
		{
			string data = string.Join("\n", list);
			var listBuilder = new DiscordEmbedBuilder();
			listBuilder
				.WithAuthor($"List of : {name}")
				.WithFooter($"「sisbase」・ {General.GetVersion()}", "https://i.imgur.com/6ovRzR9.png")
				.WithDescription(string.IsNullOrWhiteSpace(data) ? "No data" : data)
				.WithColor(DiscordColor.Orange);
			return listBuilder.Build();
		}

		/// <summary>
		/// Generates a new command help embed from a specified command.
		/// </summary>
		/// <param name="command">The command</param>
		/// <returns></returns>
		public static DiscordEmbed CommandHelpEmbed(Command command) {
			var overloads = command.Overloads;
			var usages = overloads?.Select(overload => overload.GetUsageString(command));
			var arguments = overloads?.SelectMany(x => x.Arguments)
				.Distinct()
				.Select(x => $"`{x.Name}` - **{x.Description}**");
			var embed = new DiscordEmbedBuilder()
				.WithAuthor($"Command : {command.Name} | Help")
				.WithDescription($"Usage : {string.Join(" ", usages)}")
				.WithFooter($"「sisbase」・ {General.GetVersion()}", "https://i.imgur.com/6ovRzR9.png")
				.WithColor(DiscordColor.Gray);
			if (arguments.Any())
				embed.AddField("Arguments", string.Join("\n", arguments));
			return embed.Build();
		}

		internal static string FormatCommandArgs(this IReadOnlyList<CommandArgument> args) {
			if (!args.Any()) return string.Empty;
			return string.Join(" ", args.Select(arg => $"`{arg.Name}`"));
		}

		internal static string GetUsageString(this CommandOverload overload, Command c) {
			if (overload.Arguments.FormatCommandArgs() == string.Empty) return $"[{c.Name}]";
			return $"[{c.Name} {FormatCommandArgs(overload.Arguments)}]";
		}

		/// <summary>
		/// Mutates a given <see cref="DiscordEmbed"/> with an given transformation
		/// </summary>
		/// <param name="embed">The embed that will be mutated</param>
		/// <param name="func">The transformation</param>
		/// <returns></returns>
		public static DiscordEmbed Mutate(this DiscordEmbed embed, Action<DiscordEmbedBuilder> func)
		{
			var builder = new DiscordEmbedBuilder(embed);
			func(builder);
			return builder.Build();
		}
	}
}