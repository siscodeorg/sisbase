using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using sisbase.Interactivity;
using sisbase.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace sisbase.Test.Commands
{
	[Group("test"), Description("The testbed commands")]
	public class testbed : BaseCommandModule
	{

        [GroupCommand]
        public async Task gcmd(CommandContext ctx) =>
            await ctx.RespondAsync(embed: EmbedBase.GroupHelpEmbed(ctx.Command));
        [Command("gnmdm")]
        public async Task GetNextMessageDm(CommandContext ctx)
        {
            var dmc = await ctx.Member.CreateDmChannelAsync();
            var msg = await dmc.SendMessageAsync("nmdm : WAITING");
            var response = await dmc.GetNextMessageAsync(ctx.Member);
            await msg.ModifyAsync($"ndmm : \n{response.Result.Content}");
        }

        [Command("mbuild")]
		public async Task MessBuilder(CommandContext ctx)
		{
			var mbuilder = new MessageBuilder();
			mbuilder = mbuilder
				.WithEmbed(EmbedBase.OutputEmbed("Sample Embed"))
				.WithContent("Content");
			await mbuilder.Build(ctx.Channel);
			mbuilder = mbuilder.WithEmbed(mbuilder.Embed.Mutate
				(x =>
					x.WithDescription("Mutated Embed")
						.WithColor(DiscordColor.Red))
				);
			await mbuilder.Build(ctx.Channel);
			mbuilder = mbuilder.WithContent("Mutated Content");
			await mbuilder.Build(ctx.Channel);
		}
        [Command("interact")]
        public async Task interactCmd(CommandContext ctx)
        {
            var interaction = new Interaction(ctx.Message);
            await interaction.SendMessageAsync(new MessageBuilder().WithContent("Type Anything"));
            var msg = await interaction.GetUserResponseAsync();
            await interaction.ModifyLastMessage(x => x.WithContent($"Your message were : {msg.Content}"));
            await interaction.Close();
        }

        [Command("interact3")]
        public async Task interact3Cmd(CommandContext ctx)
        {
            var interaction = new Interaction(ctx.Message);
            var msg = new MessageBuilder()
                .WithEmbed(EmbedBase.InputEmbed("Something: [1/3]"));
            await interaction.SendMessageAsync(msg);
            await interaction.GetUserResponseAsync();
            await interaction.ModifyLastMessage(x => x.WithEmbed(EmbedBase.InputEmbed("Something: [2/3]")));
            await interaction.GetUserResponseAsync();
            await interaction.ModifyLastMessage(x => x.WithEmbed(EmbedBase.InputEmbed("Something: [3/3]")));
            await interaction.GetUserResponseAsync();
            await interaction.ModifyLastMessage(x => x.WithEmbed(EmbedBase.ListEmbed(interaction.UserMessages.Select(x => x.Content).ToList(), "User Messages")));
            await interaction.Close();
        }

        [Command("interactHook")]
        public async Task interactHookCmd(CommandContext ctx)
        {
            var msg = new MessageBuilder()
                .WithEmbed(EmbedBase.InputEmbed("Whatever that includes \"aniki\", If you message doesn't the bot will ignore"));
            var interact = await ctx.Channel.WaitForInteraction(msg, x => x.Content.ToLower().Contains("aniki"));
            msg.WithEmbed(EmbedBase.OutputEmbed(interact.UserMessages.Last().Content).Mutate(x => x.WithColor(DiscordColor.Orange)));
            await msg.Build(ctx.Channel);
            await interact.Close();
        }

        [Command("interactTimeout")]
        public async Task interactTimeoutCmd(CommandContext ctx)
        {
            var interaction = new Interaction(ctx.Message) { MessageTimeout = TimeSpan.FromSeconds(5) };
            var msg = new MessageBuilder()
                .WithEmbed(EmbedBase.InputEmbed("This message will self-destruct if you do not say \"onii-chan daisuki\" within the next 5 seconds"));
            await interaction.SendMessageAsync(msg);
            var resp = await interaction.GetUserResponseAsync(x => x.Content.Contains("onii-chan daisuki"));
            if (resp == null)
                await interaction.ModifyLastMessage(m =>
                    m.WithEmbed(EmbedBase.InputEmbed("I wonder what this used to say?")));
            else
                await interaction.SendMessageAsync(new MessageBuilder().WithContent("Kyaah, kawaii~!"));
        }
        [Command("interactEvents")]
        public async Task interactEvents(CommandContext ctx)
        {
            var interaction = new Interaction(ctx.Message);
            var msg = new MessageBuilder().WithContent("Please edit your original message");
            await interaction.SendMessageAsync(msg);
            interaction.OriginDeleted += async (e) => await interaction.SendMessageAsync(new MessageBuilder().WithContent("Origin message deleted"));
            interaction.OriginEdited += async (e) => await interaction.SendMessageAsync(new MessageBuilder().WithContent($"Origin message edited : \n{e.MessageBefore?.Content} -> {e.Message.Content}"));
            interaction.OriginReactionAdded += async (e) => await interaction.SendMessageAsync(new MessageBuilder().WithContent($"Origin message [+] reaction : {e.Emoji} | {e.User.Username}"));
            interaction.OriginReactionRemoved += async (e) => await interaction.SendMessageAsync(new MessageBuilder().WithContent($"Origin message [-] reaction : {e.Emoji} | {e.User.Username}"));
            var res = await interaction.GetUserResponseAsync();
            await interaction.SendMessageAsync(msg.WithContent("Command Closed"));
            await interaction.Close();
        }

        [Command("lifetimeCrash")]
        public async Task lifetimeCrash(CommandContext ctx)
        {
	        var intr = ctx.AsInteraction();
	        intr.SetLifetime(TimeSpan.FromSeconds(10));
	        intr.OnClose += async () =>
		        await intr.SendMessageAsync(new MessageBuilder().WithContent("I'm alive!"));
	        await intr.CompletionTask();
        }

        [Command("fastCrash")]
        public async Task fastCrash(CommandContext ctx)
        {
	        var intr = ctx.AsInteraction();
	        intr.OnClose += async () =>
		        await intr.SendMessageAsync(new MessageBuilder().WithContent("I'm alive!"));
	        await intr.Close();
	        await intr.SendMessageAsync(new MessageBuilder().WithContent("I should be dead!"));
	        Logger.Log("fastCrash", "You should never see this");
        }

        [Command("endReason")]
        public async Task endReason(CommandContext ctx)
        {
	        var intr = ctx.AsInteraction();
	        intr.SetLifetime(TimeSpan.FromSeconds(3));
	        DiscordMessage resp;
	        intr.OnClose += async () =>
		        await intr.SendMessageAsync(new MessageBuilder().WithContent("Interaction closed!"));
	        try
	        {
		        resp = await intr.GetUserResponseAsync();
		        await intr.SendMessageAsync(new MessageBuilder().WithContent("you said: " + resp.Content));
	        }
	        catch (OperationCanceledException)
	        {
		        await ctx.RespondAsync("The interaction has been destroyed by the ravages of time!");
	        }
        }

        [Command("firstInt")]
		public async Task fisrtInt(CommandContext ctx, [RemainingText] string input)
			=> await ctx.RespondAsync(embed: EmbedBase.OutputEmbed($"firstInt : {ctx.Message.FirstInt()}"));

		[Command("firstEmoji")]
		public async Task fisrtEmoji(CommandContext ctx, [RemainingText] string input)
			=> await ctx.RespondAsync(embed: EmbedBase.OutputEmbed($"firstEmoji : {ctx.Message.FirstEmoji() ?? "NO EMOJI FOUND"} `{ctx.Message.FirstEmoji() ?? "NO EMOJI FOUND"}`"));
		[Command("allPerms")]
		public async Task allPerms(CommandContext ctx)
			=> await ctx.RespondAsync(embed: EmbedBase.OutputEmbed(ctx.Member.GetPermissions().ToPermissionString()));
		[Command("getPerms")]
		public async Task getPerms(CommandContext ctx, DiscordMember member)
			=> await ctx.RespondAsync(embed: EmbedBase.OutputEmbed(member.GetPermissions().ToPermissionString()));
		[Command("isMod")]
		public async Task isMod(CommandContext ctx)
			=> await ctx.RespondAsync(embed: EmbedBase.OutputEmbed(ctx.Member.IsModerator() ? "TRUE" : "FALSE"));
		[Command("isMod")]
		public async Task isMod(CommandContext ctx, DiscordMember member)
			=> await ctx.RespondAsync(embed: EmbedBase.OutputEmbed(member.IsModerator() ? "TRUE" : "FALSE"));
	}

	[Group("stubgroup")]
	public class stub : BaseCommandModule
	{
		[GroupCommand()]
		public async Task stubCmd(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.GroupHelpEmbed(ctx.Command));
	}
}