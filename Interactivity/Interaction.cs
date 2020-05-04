using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using sisbase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sisbase.Interactivity
{
#nullable enable

	public class Interaction
	{
		public List<DiscordMessage> BotMessages { get; } = new List<DiscordMessage>();

		#region Delegates
		/// All LastUserMessage Events
		private AsyncEvent<MessageReactionAddEventArgs> _lastUserMessageReactAdd;
		private AsyncEvent<MessageReactionRemoveEventArgs> _lastUserMessageReactRemove;
		private AsyncEvent<MessageUpdateEventArgs> _lastUserMessageEdit;
		private AsyncEvent<MessageDeleteEventArgs> _lastUserMessageDelete;

		/// All LastBotMessage Events
		private AsyncEvent<MessageReactionAddEventArgs> _lastBotMessageReactAdd;
		private AsyncEvent<MessageReactionRemoveEventArgs> _lastBotMessageReactRemove;
		private AsyncEvent<MessageUpdateEventArgs> _lastBotMessageEdit;
		private AsyncEvent<MessageDeleteEventArgs> _lastBotMessageDelete;

		/// All Origin Events
		private AsyncEvent<MessageReactionAddEventArgs> _originReactAdd;
		private AsyncEvent<MessageReactionRemoveEventArgs> _originReactRemove;
		private AsyncEvent<MessageUpdateEventArgs> _originEdit;
		private AsyncEvent<MessageDeleteEventArgs> _originDelete;

		/// All Message Events
		private AsyncEvent<MessageReactionAddEventArgs> _messageReactAdd;
		private AsyncEvent<MessageReactionRemoveEventArgs> _messageReactRemove;
		private AsyncEvent<MessageUpdateEventArgs> _messageEdit;
		private AsyncEvent<MessageDeleteEventArgs> _messageDelete;

		/// All Interaction Events
		private AsyncEvent _onClose;
		#endregion Delegates
		#region Events
		public event AsyncEventHandler<MessageReactionAddEventArgs> LastUserMessageReactionAdded
		{
			add => _lastUserMessageReactAdd.Register(value);
			remove => _lastUserMessageReactAdd.Unregister(value);
		}

		public event AsyncEventHandler<MessageReactionRemoveEventArgs> LastUserMessageReactionRemoved
		{
			add => _lastUserMessageReactRemove.Register(value);
			remove => _lastUserMessageReactRemove.Unregister(value);
		}

		public event AsyncEventHandler<MessageUpdateEventArgs> LastUserMessageEdited
		{
			add => _lastUserMessageEdit.Register(value);
			remove => _lastUserMessageEdit.Unregister(value);	
		}

		public event AsyncEventHandler<MessageDeleteEventArgs> LastUserMessageDeleted
		{
			add => _lastUserMessageDelete.Register(value);
			remove => _lastUserMessageDelete.Unregister(value);
		}

		public event AsyncEventHandler<MessageReactionAddEventArgs> LastBotMessageReactionAdded
		{
			add => _lastBotMessageReactAdd.Register(value);
			remove => _lastBotMessageReactAdd.Unregister(value);
		}

		public event AsyncEventHandler<MessageReactionRemoveEventArgs> LastBotMessageReactionRemoved
		{
			add => _lastBotMessageReactRemove.Register(value);
			remove => _lastBotMessageReactRemove.Unregister(value);
		}

		public event AsyncEventHandler<MessageUpdateEventArgs> LastBotMessageEdited
		{
			add => _lastBotMessageEdit.Register(value);
			remove => _lastBotMessageEdit.Unregister(value);
		}

		public event AsyncEventHandler<MessageDeleteEventArgs> LastBotMessageDeleted
		{
			add => _lastBotMessageDelete.Register(value);
			remove => _lastBotMessageDelete.Unregister(value);
		}

		public event AsyncEventHandler<MessageReactionAddEventArgs> OriginReactionAdded
		{
			add => _originReactAdd.Register(value);
			remove => _originReactAdd.Unregister(value);
		}

		public event AsyncEventHandler<MessageReactionRemoveEventArgs> OriginReactionRemoved
		{
			add => _originReactRemove.Register(value);
			remove => _originReactRemove.Unregister(value);
		}

		public event AsyncEventHandler<MessageUpdateEventArgs> OriginEdited
		{
			add => _originEdit.Register(value);
			remove => _originEdit.Unregister(value);
		}

		public event AsyncEventHandler<MessageDeleteEventArgs> OriginDeleted
		{
			add => _originDelete.Register(value);
			remove => _originDelete.Unregister(value);
		}

		public event AsyncEventHandler<MessageReactionAddEventArgs> MessageReactionAdded
		{
			add => _messageReactAdd.Register(value);
			remove => _messageReactAdd.Unregister(value);
		}

		public event AsyncEventHandler<MessageReactionRemoveEventArgs> MessageReactionRemoved
		{
			add => _messageReactRemove.Register(value);
			remove => _messageReactRemove.Unregister(value);
		}

		public event AsyncEventHandler<MessageUpdateEventArgs> MessageEdited
		{
			add => _messageEdit.Register(value);
			remove => _messageEdit.Unregister(value);
		}

		public event AsyncEventHandler<MessageDeleteEventArgs> MessageDeleted
		{
			add => _messageDelete.Register(value);
			remove => _messageDelete.Unregister(value);
		}

		public event AsyncEventHandler OnClose
		{
			add => _onClose.Register(value);
			remove => _onClose.Unregister(value);
		}
		#endregion Events
		#region Event Dispatchers
		private async Task LastMessageDispatch(MessageReactionAddEventArgs e)
			=> await _lastUserMessageReactAdd.InvokeAsync(e);
		private async Task LastMessageDispatch(MessageReactionRemoveEventArgs e)
			=> await _lastUserMessageReactRemove.InvokeAsync(e);
		private async Task LastMessageDispatch(MessageUpdateEventArgs e)
			=> await _lastUserMessageEdit.InvokeAsync(e);
		private async Task LastMessageDispatch(MessageDeleteEventArgs e)
			=> await _lastUserMessageDelete.InvokeAsync(e);

		private async Task LastBotMessageDispatch(MessageReactionAddEventArgs e)
			=> await _lastBotMessageReactAdd.InvokeAsync(e);
		private async Task LastBotMessageDispatch(MessageReactionRemoveEventArgs e)
			=> await _lastBotMessageReactRemove.InvokeAsync(e);
		private async Task LastBotMessageDispatch(MessageUpdateEventArgs e)
			=> await _lastBotMessageEdit.InvokeAsync(e);
		private async Task LastBotMessageDispatch(MessageDeleteEventArgs e)
			=> await _lastBotMessageDelete.InvokeAsync(e);

		private async Task OriginDispatch(MessageReactionAddEventArgs e)
			=> await _originReactAdd.InvokeAsync(e);
		private async Task OriginDispatch(MessageReactionRemoveEventArgs e)
			=> await _originReactRemove.InvokeAsync(e);
		private async Task OriginDispatch(MessageUpdateEventArgs e)
			=> await _originEdit.InvokeAsync(e);
		private async Task OriginDispatch(MessageDeleteEventArgs e)
			=> await _originDelete.InvokeAsync(e);

		private async Task MessageDispatch(MessageReactionAddEventArgs e)
			=> await _messageReactAdd.InvokeAsync(e);
		private async Task MessageDispatch(MessageReactionRemoveEventArgs e)
			=> await _messageReactRemove.InvokeAsync(e);
		private async Task MessageDispatch(MessageUpdateEventArgs e)
			=> await _messageEdit.InvokeAsync(e);
		private async Task MessageDispatch(MessageDeleteEventArgs e)
			=> await _messageDelete.InvokeAsync(e);

		private async Task CloseDispatch()
			=> await _onClose.InvokeAsync();
		#endregion
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
			_lastUserMessageReactAdd = new AsyncEvent<MessageReactionAddEventArgs>(HandleExceptions, "LAST_USER_MESSAGE_REACTION_ADDED");
			_lastUserMessageReactRemove = new AsyncEvent<MessageReactionRemoveEventArgs>(HandleExceptions, "LAST_USER_MESSAGE_REACTION_REMOVED");
			_lastUserMessageEdit = new AsyncEvent<MessageUpdateEventArgs>(HandleExceptions, "LAST_USER_MESSAGE_EDIT");
			_lastUserMessageDelete = new AsyncEvent<MessageDeleteEventArgs>(HandleExceptions, "LAST_USER_MESSAGE_DELETE");
			_lastBotMessageReactAdd = new AsyncEvent<MessageReactionAddEventArgs>(HandleExceptions, "LAST_BOT_MESSAGE_REACTION_ADDED");
			_lastBotMessageReactRemove = new AsyncEvent<MessageReactionRemoveEventArgs>(HandleExceptions, "LAST_BOT_MESSAGE_REACTION_REMOVED");
			_lastBotMessageEdit = new AsyncEvent<MessageUpdateEventArgs>(HandleExceptions, "LAST_BOT_MESSAGE_EDIT");
			_lastBotMessageDelete = new AsyncEvent<MessageDeleteEventArgs>(HandleExceptions, "LAST_BOT_MESSAGE_DELETE");
			_messageReactAdd = new AsyncEvent<MessageReactionAddEventArgs>(HandleExceptions, "SISBASE_MESSAGE_REACTION_ADDED");
			_messageReactRemove = new AsyncEvent<MessageReactionRemoveEventArgs>(HandleExceptions, "SISBASE_MESSAGE_REACTION_REMOVED");
			_messageEdit = new AsyncEvent<MessageUpdateEventArgs>(HandleExceptions, "SISBASE_MESSAGE_EDIT");
			_messageDelete = new AsyncEvent<MessageDeleteEventArgs>(HandleExceptions, "SISBASE_MESSAGE_DELETE");
			_originReactAdd = new AsyncEvent<MessageReactionAddEventArgs>(HandleExceptions, "ORIGIN_MESSAGE_REACTION_ADDED");
			_originReactRemove = new AsyncEvent<MessageReactionRemoveEventArgs>(HandleExceptions, "ORIGIN_MESSAGE_REACTION_REMOVED");
			_originEdit = new AsyncEvent<MessageUpdateEventArgs>(HandleExceptions, "ORIGIN_MESSAGE_EDIT");
			_originDelete = new AsyncEvent<MessageDeleteEventArgs>(HandleExceptions, "ORIGIN_MESSAGE_DELETE");
			_onClose = new AsyncEvent(HandleExceptions, "INTERACTION_CLOSED");
			IMC.AddInteraction(this);
		}

		private void HandleExceptions(string eventName, Exception ex)
		{
			if (ex is AggregateException age)
				age.Handle(x => { HandleExceptions(eventName, x); return false; });
			else
				Logger.Warn("InteractionAPI", $"An {ex.GetType()} happened in {eventName}."); 
		}

		public async Task SendMessageAsync(MessageBuilder message)
		{
			var msg = await message.Build(Origin.Channel);
			BotMessages.Add(msg);
		}

		public async Task<DiscordMessage> GetUserResponseAsync()
		{
			var msg = await UserMessages.Last().GetNextMessageAsync(MessageTimeout);
			UserMessages.Add(msg.Result);
			return msg.Result;
		}

		public async Task<DiscordMessage> GetUserResponseAsync(Func<DiscordMessage, bool> filter)
		{
			var msg = await UserMessages.Last().GetNextMessageAsync(filter, MessageTimeout);
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

		public async Task Dispatch(MessageReactionAddEventArgs e)
		{
			if(e.Message == UserMessages.LastOrDefault())
			{
				await LastMessageDispatch(e);
			}
			if(e.Message == BotMessages.LastOrDefault())
			{
				await LastBotMessageDispatch(e);
			}
			if(e.Message == Origin)
			{
				await OriginDispatch(e);
			}
			await MessageDispatch(e);
		}

		public async Task Dispatch(MessageReactionRemoveEventArgs e)
		{
			if (e.Message == UserMessages.LastOrDefault())
			{
				await LastMessageDispatch(e);
			}
			if (e.Message == BotMessages.LastOrDefault())
			{
				await LastBotMessageDispatch(e);
			}
			if (e.Message == Origin)
			{
				await OriginDispatch(e);
			}
			await MessageDispatch(e);
		}

		public async Task Dispatch(MessageUpdateEventArgs e)
		{
			if (e.Message == UserMessages.LastOrDefault())
			{
				await LastMessageDispatch(e);
			}
			if (e.Message == BotMessages.LastOrDefault())
			{
				await LastBotMessageDispatch(e);
			}
			if (e.Message == Origin)
			{
				await OriginDispatch(e);
			}
			await MessageDispatch(e);
		}

		public async Task Dispatch(MessageDeleteEventArgs e)
		{
			if (e.Message == UserMessages.LastOrDefault())
			{
				await LastMessageDispatch(e);
			}
			if (e.Message == BotMessages.LastOrDefault())
			{
				await LastBotMessageDispatch(e);
			}
			if (e.Message == Origin)
			{
				await OriginDispatch(e);
			}
			await MessageDispatch(e);
		}
		public async Task Close()
		{
			await CloseDispatch();
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
	}
}
