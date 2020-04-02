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
		public static DiscordEmbed GroupHelpEmbed(Command Command)
		{
			var commands = new List<Command>();
			CommandGroup cG = null;
			if (Command is CommandGroup cGroup)
			{
				commands = cGroup.Children.ToList();
				cG = cGroup;
			}
			string commandList = "";
			foreach (var command in commands)
			{
				commandList += $"{command.Name} - {command.Description}\n";
			}
			var groupHelpEmbed = new DiscordEmbedBuilder();
			groupHelpEmbed
				.WithFooter($"「sisbase」・ {General.GetVersion()}", "https://i.imgur.com/6ovRzR9.png")
				.WithDescription(cG?.Description)
				.AddField("Commands", string.IsNullOrWhiteSpace(commandList) ? "No sub-commands found" : commandList)
				.WithAuthor($"Group : {cG?.Name} | Help")
				.WithColor(DiscordColor.Gray);
			return groupHelpEmbed.Build();
		}

		public static async Task<DiscordEmbed> HelpEmbed(this CommandsNextExtension cne, CommandContext ctx, bool showHidden = false)
		{
			var x = cne.RegisteredCommands.Values.ToList();
			var groups = new List<CommandGroup>();
			foreach (var command in x)
			{
				if (command is CommandGroup group)
				{
					if ((await group.RunChecksAsync(ctx, true)).Count() > 0) continue;
					if (group.IsHidden && !showHidden) continue;
					groups.Add(group);
				}
			}
			var helpBuilder = new DiscordEmbedBuilder();
			foreach (var commandGroup in groups)
			{
				var children = commandGroup.Children.ToList();
				foreach (var command in children)
				{
					x.Remove(command);
				}

				x.Remove(commandGroup);
				var attributes = commandGroup.CustomAttributes.ToList();
				foreach (var y in attributes)
				{
					if (!(y is EmojiAttribute emoji)) continue;
					helpBuilder.AddField($"{emoji.Emoji} ・ {commandGroup.Name}", commandGroup.Description);
					break;
				}
			}

			string misc = "";
			foreach (var command in x)
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

		public static DiscordEmbed InputEmbed(string input)
		{
			var inputEmbedBuilder = new DiscordEmbedBuilder();
			inputEmbedBuilder
				.WithFooter($"「sisbase」・ {General.GetVersion()}", "https://i.imgur.com/6ovRzR9.png")
				.WithDescription($"Please type : {input}")
				.WithColor(DiscordColor.MidnightBlue);
			return inputEmbedBuilder.Build();
		}

		public static DiscordEmbed OutputEmbed(string output)
		{
			var outputEmbedBuilder = new DiscordEmbedBuilder();
			outputEmbedBuilder
				.WithFooter($"「sisbase」・ {General.GetVersion()}", "https://i.imgur.com/6ovRzR9.png")
				.WithDescription($"{output}")
				.WithColor(DiscordColor.SpringGreen);
			return outputEmbedBuilder.Build();
		}

		public static DiscordEmbed OrderedListEmbed<T>(List<T> list, string name,
			CountingBehaviour behaviour = CountingBehaviour.Default)
		{
			string data = "";
			foreach (var item in list)
			{
				if (behaviour == CountingBehaviour.Ordinal) data += $"{list.IndexOf(item) + 1}・{item.ToString()}\n";
				else data += $"{list.IndexOf(item)}・{item.ToString()}\n";
			}
			var orderedListBuilder = new DiscordEmbedBuilder();
			orderedListBuilder
				.WithAuthor($"List of : {name}")
				.WithFooter($"「sisbase」・ {General.GetVersion()}", "https://i.imgur.com/6ovRzR9.png")
				.WithDescription(string.IsNullOrWhiteSpace(data) ? "No data" : data)
				.WithColor(DiscordColor.Orange);
			return orderedListBuilder.Build();
		}

		public static DiscordEmbed ListEmbed<T>(IEnumerable<T> list, string name)
		{
			string data = list.Aggregate("", (current, item) => current + $"・{item.ToString()}\n");
			var listBuilder = new DiscordEmbedBuilder();
			listBuilder
				.WithAuthor($"List of : {name}")
				.WithFooter($"「sisbase」・ {General.GetVersion()}", "https://i.imgur.com/6ovRzR9.png")
				.WithDescription(string.IsNullOrWhiteSpace(data) ? "No data" : data)
				.WithColor(DiscordColor.Orange);
			return listBuilder.Build();
		}

		public static DiscordEmbed CommandHelpEmbed(Command command)
		{
			if (command.Overloads?.Any() == true)
			{
				string use = "";
				var o = command.Overloads.ToList();
				var arguments = new List<CommandArgument>();
				o.RemoveAll(x => x.Arguments.Count == 0);
				foreach (var overload in o)
				{
					string inner = "";
					var args = overload.Arguments.ToList();
					foreach (var argument in args)
					{
						if (!arguments.Contains(argument))
						{
							arguments.Add(argument);
						}
						inner += $"`{argument.Name}` ";
					}
					use += $"[{command.Name} {inner}] ";
				}

				string argumentExplanation = "";
				arguments.ForEach(x => argumentExplanation += $"{x.Name} - {x.Description}\n");
				var commandHelpEmbed = new DiscordEmbedBuilder();
				commandHelpEmbed
					.WithFooter($"「sisbase」・ {General.GetVersion()}", "https://i.imgur.com/6ovRzR9.png")
					.AddField("Arguments", argumentExplanation)
					.WithDescription($"Use : {use}")
					.WithAuthor($"Command : {command.Name} | Help")
					.WithColor(DiscordColor.Gray);
				return commandHelpEmbed.Build();
			}
			else
			{
				var commandHelpEmbed = new DiscordEmbedBuilder();
				commandHelpEmbed
					.WithFooter($"「sisbase」・ {General.GetVersion()}", "https://i.imgur.com/6ovRzR9.png")
					.WithDescription("This command is a stub and was not implemented yet.")
					.WithAuthor($"Command : {command.Name} | Help")
					.WithColor(DiscordColor.Gray);
				return commandHelpEmbed.Build();
			}
		}

		public static DiscordEmbed Mutate(this DiscordEmbed embed, Action<DiscordEmbedBuilder> func)
		{
			var builder = new DiscordEmbedBuilder(embed);
			func(builder);
			return builder.Build();
		}
	}
}