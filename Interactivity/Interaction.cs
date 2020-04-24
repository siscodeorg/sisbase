using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using sisbase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Interactivity
{
#nullable enable

	public class Interaction
	{
		///together, these two lists contain the history of the Interaction. their lengths should never differ by more than one.
		public List<DiscordMessage> BotMessages { get; } = new List<DiscordMessage>();

		public List<DiscordMessage> UserMessages { get; } = new List<DiscordMessage>();  // probably this begins populated with `origin`

		public DiscordMessage Origin { get; }  // this contains the User and Channel to be used as the context
		public TimeSpan MessageTimeout { get; set; }

		public Interaction(DiscordMessage origin)
		{
			Origin = origin;
			if (Origin.Author == SisbaseBot.Instance.Client.CurrentUser) throw new ArgumentException($"Origin message can't be sent by the bot itself. Consider using a message created by the used you are interacting with.", "origin");
			else UserMessages.Add(origin);
		}

		public async Task SendMessageAsync(MessageBuilder message)
		{
			var msg = await message.Build(Origin.Channel);
			BotMessages.Add(msg);
		}// send and add to botMessages

		public async Task<DiscordMessage> GetUserResponseAsync()
		{
			var msg = await UserMessages.Last().GetNextMessageAsync();
			UserMessages.Add(msg.Result);
			return msg.Result;
		}// Await message, add to userMessages, and return

		public async Task<DiscordMessage> GetUserResponseAsync(Func<DiscordMessage, bool> filter)
		{
			var msg = await UserMessages.Last().GetNextMessageAsync(filter);
			UserMessages.Add(msg.Result);
			return msg.Result;
		}

		public async Task ModifyLastMessage(Action<MessageBuilder> func)
		{
			var builder = new MessageBuilder(BotMessages.Last());
			func(builder);
			var msg = await builder.Build(BotMessages.Last().Channel);
			BotMessages.RemoveAll(x => x.Id == builder.MessageId);
			BotMessages.Add(msg);
		}// does a discordmessage contain much information about the content?

		public void Close()
		{
			MessageTimeout = TimeSpan.Zero;
			BotMessages.Clear();
			UserMessages.Clear();
		}// purely bookkeeping. this may be unnecessary?
	}

	public static class InteractionExtensions
	{
		public static async Task<Interaction> WaitForInteraction(this DiscordChannel channel,
			MessageBuilder hook,
			Func<DiscordMessage, bool> interactioncheck)
		{
			await hook.Build(channel);
			var response = await channel.GetNextMessageAsync(interactioncheck);
			return new Interaction(response.Result);
		}
	}
}