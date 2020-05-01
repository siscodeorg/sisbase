using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using sisbase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static sisbase.Interactivity.EventArgs;

namespace sisbase.Interactivity
{
#nullable enable

	public class Interaction
	{
		//TODO : 
		//Learn how D#+ AsyncEventHandler Works and implement it here.
		public List<DiscordMessage> BotMessages { get; } = new List<DiscordMessage>();

		#region Delegates
	
		public delegate MessageReactArgs onLastMessageReact(Interaction sender,MessageReactArgs args);
		public delegate MessageReactArgs onOriginMessageReact(Interaction sender, MessageReactArgs args);
		public delegate MessageReactArgs onMessageReact(Interaction sender, MessageReactArgs args);
		public delegate MessageEditArgs onLastMessageEdit(Interaction sender, MessageEditArgs args);
		public delegate MessageEditArgs onOriginMessageEdit(Interaction sender, MessageEditArgs args);
		public delegate MessageEditArgs onMessageEdit(Interaction sender, MessageEditArgs args);
		public delegate MessageDeleteArgs onLastMessageDelete(Interaction sender, MessageDeleteArgs args);
		public delegate MessageDeleteArgs onOriginMessageDelete(Interaction sender, MessageDeleteArgs args);
		public delegate MessageDeleteArgs onMessageDelete(Interaction sender, MessageDeleteArgs args);
		#endregion Delegates
		#region Events
		public event onLastMessageReact LastMessageReacted;
		public event onOriginMessageReact OriginMessageReacted;
		public event onMessageReact MessageReacted;
		public event onLastMessageEdit LastMessageEdited;
		public event onOriginMessageEdit OriginMessageEdited;
		public event onMessageEdit MessageEdited;
		public event onLastMessageDelete LastMessageDeleted;
		public event onOriginMessageDelete OriginMessageDeleted;
		public event onMessageDelete MessageDeleted;
		#endregion Events
		public List<DiscordMessage> UserMessages { get; } = new List<DiscordMessage>();
		
		public DiscordMessage Origin { get; }
		public TimeSpan? MessageTimeout { get; set; }

		public Interaction(DiscordMessage origin)
		{
			if (origin.Author == SisbaseBot.Instance.Client.CurrentUser)
			{
				throw new ArgumentException($"Origin message can't be sent by the bot itself."+
					" Consider using a message created by the used you are interacting with.", "origin");
			}
			Origin = origin;
			UserMessages.Add(origin);
			IMC.AddInteraction(this);
		}

		public async Task SendMessageAsync(MessageBuilder message)
		{
			var msg = await message.Build(Origin.Channel);
			BotMessages.Add(msg);
			IMC.UpdateInteraction(Origin, this);
		}

		public async Task<DiscordMessage> GetUserResponseAsync()
		{
			var msg = await UserMessages.Last().GetNextMessageWithTimeoutAsync(MessageTimeout);
			UserMessages.Add(msg.Result);
			IMC.UpdateInteraction(Origin, this);
			return msg.Result;
		}

		public async Task<DiscordMessage> GetUserResponseAsync(Func<DiscordMessage, bool> filter)
		{
			var msg = await UserMessages.Last().GetNextMessageWithTimeoutAsync(filter, MessageTimeout);
			UserMessages.Add(msg.Result);
			IMC.UpdateInteraction(Origin, this);
			return msg.Result;
		}

		public async Task ModifyLastMessage(Action<MessageBuilder> func)
		{
			var builder = new MessageBuilder(BotMessages.Last());
			func(builder);
			var msg = await builder.Build(BotMessages.Last().Channel);
			BotMessages.RemoveAll(x => x.Id == builder.MessageId);
			BotMessages.Add(msg);
			IMC.UpdateInteraction(Origin, this);
		}
		internal void InvokeEvent(MessageEditArgs args) 
		{
			if (args.After.Id == Origin.Id)
				OriginMessageEdited?.Invoke(this, args);
			if (args.After.Id == UserMessages.Last().Id)
				LastMessageEdited?.Invoke(this, args);
			MessageEdited?.Invoke(this, args);
		}
		internal void InvokeEvent(MessageReactArgs args)
		{
			if (args.Message.Id == Origin.Id)
				OriginMessageReacted?.Invoke(this, args);
			if (args.Message.Id == UserMessages.Last().Id)
				LastMessageReacted?.Invoke(this, args);
			MessageReacted?.Invoke(this, args);
		}
		internal void InvokeEvent(MessageDeleteArgs args)
		{
			if (args.Message.Id == Origin.Id)
				OriginMessageDeleted?.Invoke(this, args);
			if (args.Message.Id == UserMessages.Last().Id)
				LastMessageDeleted?.Invoke(this, args);
			MessageDeleted?.Invoke(this, args);
		}
		public void Close()
		{
			IMC.RemoveIntraction(this);
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
