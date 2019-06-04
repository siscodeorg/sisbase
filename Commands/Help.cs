using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using LA_RPbot.Discord.Utils;

namespace LA_RPbot.Discord.Commands
{
    public class Help : BaseCommandModule
    {
        // This is a sample help command.
        [Command("help")]
        public async Task helpCommand(CommandContext ctx)
        {
            await ctx.RespondAsync(embed:ctx.CommandsNext.HelpEmbed());
        }
    }
}