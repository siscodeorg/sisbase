using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using sisbase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;

namespace sisbase.Interactivity
{
#nullable enable

	public class Interaction
	{
		public List<DiscordMessage> BotMessages { get; } = new List<DiscordMessage>();

		public List<DiscordMessage> UserMessages { get; } = new List<DiscordMessage>();

		public DiscordMessage Origin { get; }
		public TimeSpan? MessageTimeout { get; set; }

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
			var msg = await UserMessages.Last().GetNextMessageWithTimeoutAsync(MessageTimeout);
			UserMessages.Add(msg.Result);
			return msg.Result;
		}

		public async Task<DiscordMessage> GetUserResponseAsync(Func<DiscordMessage, bool> filter)
		{
			var msg = await UserMessages.Last().GetNextMessageWithTimeoutAsync(filter, MessageTimeout);
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

		/// <summary>
		/// this exists because GetNextMessageAsync from D#+ does not propagate timeout information
		/// </summary>
		/// <param name="c"></param>
		/// <param name="predicate"></param>
		/// <param name="timeoutoverride"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		internal static async Task<InteractivityResult<DiscordMessage>> GetNextMessageWithTimeoutAsync(this DiscordChannel c, Func<DiscordMessage, bool> predicate,
			TimeSpan? timeoutoverride = null)
		{
			var interactivity = SisbaseBot.Instance.Interactivity;

			if (interactivity == null)
				throw new InvalidOperationException("Interactivity was not set up!");

			if (timeoutoverride == null)
				return await interactivity.WaitForMessageAsync(x => x.ChannelId == c.Id && predicate(x));
			else
				return await interactivity.WaitForMessageAsync(x => x.ChannelId == c.Id && predicate(x),
					timeoutoverride);
		}

		internal static async Task<InteractivityResult<DiscordMessage>> GetNextMessageWithTimeoutAsync(this DiscordMessage m, TimeSpan? timeoutoverride = null)
			=> await m.Channel.GetNextMessageWithTimeoutAsync(x => x.Author.Id == m.Author.Id && m.ChannelId == x.ChannelId, timeoutoverride);

		internal static async Task<InteractivityResult<DiscordMessage>> GetNextMessageWithTimeoutAsync(this DiscordMessage m, Func<DiscordMessage, bool> predicate, 
			TimeSpan? timeoutoverride = null)
			=> await m.Channel.GetNextMessageWithTimeoutAsync(x => x.Author.Id == m.Author.Id && m.ChannelId == x.ChannelId && predicate(x), timeoutoverride);
	}
}
