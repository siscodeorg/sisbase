using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using sisbase.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;

namespace sisbase.Interactivity {
#nullable enable

	public class Interaction : IDisposable {
		internal List<InteractionMessage> _botMessages { get; } = new List<InteractionMessage>();
		internal List<InteractionMessage> _userMessages { get; } = new List<InteractionMessage>();
		public InteractionMessageListProxy BotMessages { get; }
		public InteractionMessageListProxy UserMessages { get; }
		public DiscordMessage Origin { get; }
		public TimeSpan? MessageTimeout { get; set; }

		private readonly CancellationTokenSource _lifetime = new CancellationTokenSource();
		public bool IsAlive => !_lifetime.IsCancellationRequested;
		private readonly object _closingLock = new object();
		private bool _isClosing;
		private bool _isClosed;

		private readonly AsyncEvent _onClose;
		public event AsyncEventHandler InteractionClosed {
			add => _onClose.Register(value);
			remove => _onClose.Unregister(value);
		}
		public async Task Dispatch() => await _onClose.InvokeAsync();
		public Interaction(DiscordMessage origin) {
			if (origin.Author == SisbaseBot.Instance.Client.CurrentUser) {
				throw new ArgumentException($"Origin message can't be sent by the bot itself." +
					" Consider using a message created by the used you are interacting with.", "origin");
			}
			Origin = origin;
			_userMessages.Add(new InteractionMessage(origin, this));
			_lifetime.Token.Register(() => Task.Run(async () => await Close()).Wait());
			SetLifetime(TimeSpan.FromMinutes(5));
			_onClose = new AsyncEvent(IMC.HandleExceptions, "INTERACTION_CLOSED");
			BotMessages = new InteractionMessageListProxy(Enums.InteractionMessageListProxyMode.BOT, this);
			UserMessages = new InteractionMessageListProxy(Enums.InteractionMessageListProxyMode.USER, this);
			IMC.AddInteraction(this);

		}
		public async Task<InteractionMessage> SendMessageAsync(MessageBuilder message) {
			LifeCheck();
			var msg = await message.Build(Origin.Channel);
			var imsg = new InteractionMessage(msg, this);
			_botMessages.Add(imsg);
			return imsg;
		}
		public async Task<InteractionMessage> SendFileAsync(MessageBuilder message, Stream data) {
			var msg = await message.Bind(data as FileStream).Build(Origin.Channel);
			var imsg = new InteractionMessage(msg, this);
			_botMessages.Add(imsg);
			return imsg;
		}
		public async Task RemoveAsync(InteractionMessage interactionMessage, string reason = "") {
			await interactionMessage._Message.DeleteAsync(reason);
			if (interactionMessage.Author == SisbaseBot.Instance.Client.CurrentUser)
				_botMessages.Remove(interactionMessage);
			else
				_userMessages.Remove(interactionMessage);
		}
		public async Task SendMessageAsync(string content)
			=> await SendMessageAsync(new MessageBuilder(content));
		public async Task SendMessageAsync(DiscordEmbed embed)
			=> await SendMessageAsync(new MessageBuilder(embed));

		public async Task<InteractionMessage> GetUserResponseAsync() {
			LifeCheck(strict: true);
			var dspmsg = await WaitNextMessageAsync((e) => true, MessageTimeout ?? default, _lifetime.Token);
			var imsg = new InteractionMessage(dspmsg, this);
			_userMessages.Add(imsg);
			return imsg;
		}

		public async Task<InteractionMessage> GetUserResponseAsync(Func<DiscordMessage, bool> filter) {
			LifeCheck(strict: true);
			var dspmsg = await WaitNextMessageAsync(filter, MessageTimeout ?? default, _lifetime.Token);
			var imsg = new InteractionMessage(dspmsg, this);
			_userMessages.Add(imsg);
			return imsg;
		}

		public async Task ModifyLastMessage(Action<MessageBuilder> func) {
			LifeCheck();
			var builder = new MessageBuilder(_botMessages.Last()._Message);
			func(builder);
			var msg = await builder.Build(_botMessages.Last().Channel);
			_botMessages.RemoveAll(x => x.Id == builder.MessageId);
			_botMessages.Add(new InteractionMessage(msg, this));
		}

		public void SetLifetime(TimeSpan time) => _lifetime.CancelAfter(time);

		private void LifeCheck(bool strict = false) {
			if (_isClosed || (strict && _isClosing)) {
				throw new OperationCanceledException(_lifetime.Token);
			}
		}

		internal async Task<DiscordMessage> WaitNextMessageAsync(Func<DiscordMessage, bool> pred, TimeSpan timeout = default, CancellationToken token = default) {
			var dspargs = await EventWaiter<MessageCreateEventArgs>.Wait(e => 
				    e.Channel == Origin.Channel && e.Author == Origin.Author && pred(e.Message)
				, timeout, token);
			return dspargs.Message;
		}

		public async Task Close() {
			lock (_closingLock) {
				if (_isClosing) return;
				_isClosing = true;
			}
			await Dispatch();
			_isClosed = true;
			_lifetime.Cancel();
			IMC.RemoveIntraction(this);
			MessageTimeout = TimeSpan.Zero;
			_botMessages.Clear();
			_userMessages.Clear();
		}
		public Task CompletionTask() => _lifetime.Token.WhenCanceled();

		public void Dispose() {
			_lifetime.Dispose();
		}
	}

	public static class InteractionExtensions {
		public static async Task<Interaction> WaitForInteraction(this DiscordChannel channel,
			MessageBuilder hook,
			Func<DiscordMessage, bool> interactioncheck) {
			await hook.Build(channel);
			var response = await channel.GetNextMessageAsync(interactioncheck);
			return new Interaction(response.Result);
		}

		public static Interaction AsInteraction(this CommandContext ctx) => new Interaction(ctx.Message);
	}
}
