
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using sisbase.Attributes;
using sisbase.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace sisbase.Commands
{
    public class Developer : BaseCommandModule
    {
        [Command("setMaster")]
        [RequireOwner]
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
            var allSystems = new List<string>();
            SMC.RegisteredSystems.ToList().ForEach(x => allSystems.Add($"{x.Value.Name} - `{x.Key.Assembly.GetName().Name}`"));
            var embed = EmbedBase.ListEmbed(allSystems, "Systems");
            await ctx.RespondAsync(embed: embed);
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
                        File.WriteAllText(SisbaseBot.Instance.SisbaseConfiguration.JsonPath ,
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