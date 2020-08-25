using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using sisbase.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Test.Commands {
    public class hierarchy : BaseCommandModule {
        [Command("above")]
        public async Task AboveCommand(CommandContext ctx, DiscordRole role)
            => await ctx.RespondAsync(embed: EmbedBase.OutputEmbed($" Is {ctx.Member.Mention}[{ctx.Member.Hierarchy}] " +
                $"Above : {role.Mention}[{role.Position}]? {ctx.Member.IsAbove(role)}"));
        [Command("above")]
        public async Task AboveCommand(CommandContext ctx, DiscordRole role, DiscordRole role2)
            => await ctx.RespondAsync(embed: EmbedBase.OutputEmbed($" Is {role.Mention}[{role.Position}] " +
                $"Above : {role2.Mention}[{role2.Position}]? {role.IsAbove(role2)}"));
        [Command("highroleabove")]
        public async Task HighRoleAboveCommand(CommandContext ctx, DiscordRole role)
           => await ctx.RespondAsync(embed: EmbedBase.OutputEmbed($" Is {ctx.Member.GetHighestRole().Mention}[{ctx.Member.GetHighestRole().Position}] " +
               $"Above : {role.Mention}[{role.Position}]? {ctx.Member.GetHighestRole().IsAbove(role)}"));
    }
}
