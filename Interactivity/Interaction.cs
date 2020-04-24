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
		public List<DiscordMessage> BotMessages { get; } = new List<DiscordMessage>();

		public List<DiscordMessage> UserMessages { get; } = new List<DiscordMessage>();

		public DiscordMessage Origin { get; }
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
		}

		public async Task<DiscordMessage> GetUserResponseAsync()
		{
			var msg = await UserMessages.Last().GetNextMessageAsync();
			UserMessages.Add(msg.Result);
			return msg.Result;
		}

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
		}

		public void Close()
		{
			MessageTimeout = TimeSpan.Zero;
			BotMessages.Clear();
			UserMessages.Clear();
		}
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