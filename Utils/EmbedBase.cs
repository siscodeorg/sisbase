using System.Collections.Generic;
using System.Linq;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace LA_RPbot.Discord.Utils
{
    public static class EmbedBase
    {

        public static DiscordEmbed GroupHelpEmbed(Command Command){
            List<Command> commands = new List<Command>();
            CommandGroup cG = null;
            if (Command is CommandGroup cGroup)
            {
                commands  = cGroup.Children.ToList();
                cG = cGroup;
            }
            var commandList = "";
            foreach (var command in commands)
            {
                commandList += $"{command.Name} - {command.Description}\n";
            }
            var groupHelpEmbed = new DiscordEmbedBuilder();
            groupHelpEmbed
                .WithFooter("「lolibase」・ 0.1","https://i.imgur.com/6ovRzR9.png")
                .WithDescription(cG?.Description)
                .AddField("Commands",string.IsNullOrWhiteSpace(commandList)?"No sub-commands found":commandList)
                .WithAuthor($"Group : {cG?.Name} | Help")
                .WithColor(DiscordColor.Gray);
            return groupHelpEmbed.Build();
        }

        public static DiscordEmbed InputEmbed(string input)
        {
            var inputEmbedBuilder = new DiscordEmbedBuilder();
            inputEmbedBuilder
                .WithFooter("「lolibase」・ 0.1","https://i.imgur.com/6ovRzR9.png")
                .WithDescription($"Please type : {input}")
                .WithColor(DiscordColor.MidnightBlue);
            return inputEmbedBuilder.Build();
        }

        public static DiscordEmbed OutputEmbed(string output)
        {
            var outputEmbedBuilder = new DiscordEmbedBuilder();
            outputEmbedBuilder
                .WithFooter("「lolibase」・ 0.1","https://i.imgur.com/6ovRzR9.png")
                .WithDescription($"{output}")
                .WithColor(DiscordColor.SpringGreen);
            return outputEmbedBuilder.Build();
        }

        public static DiscordEmbed OrderedListEmbed<T>(List<T> list, string name)
        {
            var data = "";
            foreach (var item in list)
            {
                data += $"{list.IndexOf(item)}・{item.ToString()}\n";
            }
            var orderedListBuilder = new DiscordEmbedBuilder();
            orderedListBuilder
                .WithAuthor($"List of : {name}")
                .WithFooter("「lolibase」・ 0.1","https://i.imgur.com/6ovRzR9.png")
                .WithDescription(string.IsNullOrWhiteSpace(data)?"No data":data)
                .WithColor(DiscordColor.Orange);
            return orderedListBuilder.Build();
        }

        public static DiscordEmbed ListEmbed<T>(IEnumerable<T> list, string name)
        {
            var data = list.Aggregate("", (current, item) => current + $"・{item.ToString()}\n");
            var listBuilder = new DiscordEmbedBuilder();
            listBuilder
                .WithAuthor($"List of : {name}")
                .WithFooter("「lolibase」・ 0.1","https://i.imgur.com/6ovRzR9.png")
                .WithDescription(string.IsNullOrWhiteSpace(data)?"No data":data)
                .WithColor(DiscordColor.Orange);
            return listBuilder.Build();
        }
        public static DiscordEmbed CommandHelpEmbed(Command command)
        {
            if (command.Overloads?.Any() == true)
            {
                var use= "";
                List<CommandOverload> o = command.Overloads.ToList();
                var arguments = new List<CommandArgument>();
                o.RemoveAll(x => x.Arguments.Count == 0);
                foreach (var overload in o)
                {
                    var inner = "";
                    List<CommandArgument> args = overload.Arguments.ToList();
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

                var argumentExplanation = "";
                arguments.ForEach(x => argumentExplanation += $"{x.Name} - {x.Description}\n");
                var commandHelpEmbed = new DiscordEmbedBuilder();
                commandHelpEmbed
                    .WithFooter("「lolibase」・ 0.1","https://i.imgur.com/6ovRzR9.png")
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
                    .WithFooter("「lolibase」・ 0.1", "https://i.imgur.com/6ovRzR9.png")
                    .WithDescription("This command is a stub and was not implemented yet.")
                    .WithAuthor($"Command : {command.Name} | Help")
                    .WithColor(DiscordColor.Gray);
                return commandHelpEmbed.Build();
            }
        }
    }
}