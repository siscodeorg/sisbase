using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using sisbase.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static sisbase.Utils.Behaviours;

namespace sisbase.Utils
{
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
			var RegisteredCommands = cne.RegisteredCommands.Values.ToList();
			var groups = new List<CommandGroup>();
			foreach (var command in RegisteredCommands)
			{
				if (command is CommandGroup group)
				{
					if (groups.Contains(group)) continue;
					if ((await group.RunChecksAsync(ctx, true)).Count() > 0) continue;
					if (group.IsHidden && !showHidden) continue;
					groups.Add(group);
				}
			}
			var helpBuilder = new DiscordEmbedBuilder();
			var unk = DiscordEmoji.FromName(SisbaseBot.Instance.Client, ":grey_question:");
			foreach (var commandGroup in groups)
			{
				var children = commandGroup.Children.ToList();
				commandGroup.Children.ToList().ForEach(x => RegisteredCommands.Remove(x));
				RegisteredCommands.Remove(commandGroup);
				var attributes = commandGroup.CustomAttributes.ToList();
				bool HasEmoji = attributes.Any(x => x is EmojiAttribute);
				var emoji = HasEmoji ? ((EmojiAttribute)attributes.Where(x => x is EmojiAttribute).First()).Emoji : unk;
				helpBuilder.AddField($"{emoji} ・ {commandGroup.Name}", !string.IsNullOrWhiteSpace(commandGroup.Description) ? commandGroup.Description : $"{commandGroup.Name} - STUB : Doesn't have a description.");
			}

			string misc = "";
			foreach (var command in RegisteredCommands)
			{
				if ((await command.RunChecksAsync(ctx, true)).Count() > 0) continue;
				if (command.IsHidden && !showHidden) continue;
				misc += $"`{command.Name}` ";
			}

			helpBuilder.AddField("❓ ・ Miscellaneous ", misc);
			helpBuilder
				.WithDescription($"To see help for a group run {SisbaseBot.Instance.Client.CurrentUser.Mention} `group name`")
				.WithFooter($"「sisbase」・ {General.GetVersion()}", "https://i.imgur.com/6ovRzR9.png")
				.WithAuthor("Help | Showing all groups")
				.WithColor(DiscordColor.CornflowerBlue);
			return helpBuilder.Build();
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