using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using LA_RPbot.Discord.Attributes;
using LA_RPbot.Discord.Utils;
using Newtonsoft.Json;

namespace LA_RPbot.Discord.Commands
{
    public class Developer : BaseCommandModule
    {
        [Command("setMaster")]
        [RequireOwner]
        public async Task SetMaster(CommandContext ctx)
        {
            List<DiscordGuild> guilds = Program.Client.Guilds.Values.ToList();
            var ids = new List<ulong?>();
            var embed = new DiscordEmbedBuilder();
            embed
                .WithAuthor($"Set {ctx.Guild.Name} as MASTER")
                .WithColor(DiscordColor.PhthaloGreen);

            guilds.Remove(ctx.Guild);
            Program.Config.MasterId = ctx.Guild.Id;
            guilds.ForEach(x => ids.Add(x.Id));
            Program.Config.PuppetId = ids;
            File.WriteAllText(Directory.GetCurrentDirectory() + "/Config.json",
                JsonConvert.SerializeObject(Program.Config, Formatting.Indented));
            await ctx.RespondAsync(embed: embed);
        }
    }

    [OniiSan] // Sets command to be only executable on the master server
    [Imouto] // Sets command to be only executable by the staff (Modify Roles)
    [Emoji(":wrench:")] // Sets the emoji for the group
    [Group("config")]
    [Description("This group configures the bot.")]
    public class Config : BaseCommandModule
    {
        [GroupCommand]
        public async Task Command(CommandContext ctx)
        {
            var embed = EmbedBase.GroupHelpEmbed(ctx.Command);
            await ctx.RespondAsync(embed:embed);
        }

        [Command("prefix")]
        [Description("Changes the custom prefixes")]
        public async Task PrefixError(CommandContext ctx)
        {
            var embed = EmbedBase.CommandHelpEmbed(ctx.Command);
            await ctx.RespondAsync(embed:embed);
        }

        // Sample of a command that uses interactivity and lolibase to input/output text data.
        [Command("prefix")]
        public async Task PrefixSuccess(CommandContext ctx, [Description("The operation to be executed [add/list/del] ")]
            string operation)
        {
            switch (operation.ToLowerInvariant())
            {
                case "add":
                    var msg = await ctx.RespondAsync(embed: EmbedBase.InputEmbed("Prefix to be added"));
                    InteractivityResult<DiscordMessage> response = await ctx.Message.GetNextMessageAsync();
                    var prefix =  response.Result.Content;
                    if (Program.Config.Prefixes.Contains(prefix.ToLowerInvariant()))
                    {
                        await msg.ModifyAsync(embed:EmbedBase.OutputEmbed($"This prefix is already added."));
                    }
                    else
                    {
                        Program.Config.Prefixes.Add(prefix.ToLowerInvariant());
                        File.WriteAllText(Directory.GetCurrentDirectory() + "/Config.json",
                            JsonConvert.SerializeObject(Program.Config, Formatting.Indented));
                        await msg.ModifyAsync(embed:EmbedBase.OutputEmbed($"Prefix added without errors."));
                    }

                    break;
                case "del":
                    var msg2 = await ctx.RespondAsync(embed: EmbedBase.InputEmbed("Prefix to be removed"));
                    InteractivityResult<DiscordMessage> response2 = await ctx.Message.GetNextMessageAsync();
                    var prefix2 =  response2.Result.Content;
                    if (!Program.Config.Prefixes.Contains(prefix2.ToLowerInvariant()))
                    {
                        await msg2.ModifyAsync(embed:EmbedBase.OutputEmbed($"This prefix doesn't exists."));
                    }
                    else
                    {
                        Program.Config.Prefixes.Remove(prefix2.ToLowerInvariant());
                        File.WriteAllText(Directory.GetCurrentDirectory() + "/Config.json",
                            JsonConvert.SerializeObject(Program.Config, Formatting.Indented));
                        await msg2.ModifyAsync(embed:EmbedBase.OutputEmbed($"Prefix removed without errors."));
                    }
                    break;
                case "list":
                    await ctx.RespondAsync(embed:EmbedBase.OrderedListEmbed(Program.Config.Prefixes,"Prefixes"));
                    break;
                default:
                    var embed = EmbedBase.CommandHelpEmbed(ctx.Command);
                    await ctx.RespondAsync(embed:embed);
                    break;
            }
        }
    }
    
}