using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using sisbase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using System.IO;

namespace sisbase.Interactivity
{
#nullable enable

	public class Interaction
	{
		public List<InteractionMessage> BotMessages { get; } = new List<InteractionMessage>();
		public List<InteractionMessage> UserMessages { get; } = new List<InteractionMessage>();

		public DiscordMessage Origin { get; }
		public TimeSpan? MessageTimeout { get; set; }

		private readonly CancellationTokenSource _lifetime = new CancellationTokenSource();
		public bool IsAlive => !_lifetime.IsCancellationRequested;
		private readonly object _closingLock = new object();
		private bool _isClosing;
		private bool _isClosed;

		public Interaction(DiscordMessage origin)
		{
			if (origin.Author == SisbaseBot.Instance.Client.CurrentUser)
			{
				throw new ArgumentException($"Origin message can't be sent by the bot itself." +
					" Consider using a message created by the used you are interacting with.", "origin");
			}
			Origin = origin;
			UserMessages.Add(new InteractionMessage(origin,this));
			_lifetime.Token.Register(() => Task.Run(async () => await Close()).Wait());
			SetLifetime(TimeSpan.FromMinutes(5));
			IMC.AddInteraction(this);
		}

		internal static void HandleExceptions(string eventName, Exception ex)
		{
			if (ex is OperationCanceledException) return;
			if (ex is AggregateException age)
				age.Handle(x => { HandleExceptions(eventName, x); return false; });
			else
				Logger.Warn("InteractionAPI", $"An {ex.GetType()} happened in {eventName}.");
		}

		public async Task<InteractionMessage> SendMessageAsync(MessageBuilder message)
		{
			LifeCheck();
			var msg = await message.Build(Origin.Channel);
			var imsg = new InteractionMessage(msg, this);
			BotMessages.Add(imsg);
			return imsg;
		}
		public async Task<InteractionMessage> SendFileAsync(MessageBuilder message, Stream data)
		{
			var msg = await message.Bind(data as FileStream).Build(Origin.Channel);
			var imsg = new InteractionMessage(msg, this);
			BotMessages.Add(imsg);
			return imsg;
		}
		public async Task RemoveAsync(InteractionMessage interactionMessage, string reason = "")
		{
			await interactionMessage._Message.DeleteAsync(reason);
			if (interactionMessage.Author == SisbaseBot.Instance.Client.CurrentUser)
				BotMessages.Remove(interactionMessage);
			else
				UserMessages.Remove(interactionMessage);
		}
		public async Task SendMessageAsync(string content)
			=> await SendMessageAsync(new MessageBuilder(content));
		public async Task SendMessageAsync(DiscordEmbed embed)
			=> await SendMessageAsync(new MessageBuilder(embed));

		public async Task<DiscordMessage> GetUserResponseAsync()
		{
			LifeCheck(strict: true);
			var msg = await UserMessages.Last()._Message.GetNextMessageAsync(MessageTimeout).DetachOnCancel(_lifetime.Token);
			UserMessages.Add(new InteractionMessage(msg.Result,this));
			return msg.Result;
		}

		public async Task<DiscordMessage> GetUserResponseAsync(Func<DiscordMessage, bool> filter)
		{
			LifeCheck(strict: true);
			var msg = await UserMessages.Last()._Message.GetNextMessageAsync(filter, MessageTimeout).DetachOnCancel(_lifetime.Token);
			UserMessages.Add(new InteractionMessage(msg.Result,this));
			return msg.Result;
		}

		public async Task ModifyLastMessage(Action<MessageBuilder> func)
		{
			LifeCheck();
			var builder = new MessageBuilder(BotMessages.Last()._Message);
			func(builder);
			var msg = await builder.Build(BotMessages.Last().Channel);
			BotMessages.RemoveAll(x => x.Id == builder.MessageId);
			BotMessages.Add(new InteractionMessage(msg,this));
		}

		public void SetLifetime(TimeSpan time)
		{
			_lifetime.CancelAfter(time);
		}

		private void LifeCheck(bool strict = false)
		{
			if (_isClosed || (strict && _isClosing))
			{
				throw new OperationCanceledException(_lifetime.Token);
			}
		}

		public async Task Close()
		{
			lock (_closingLock)
			{
				if (_isClosing) return;
				_isClosing = true;
			}
			_isClosed = true;
			_lifetime.Cancel();
			IMC.RemoveIntraction(this);
			MessageTimeout = TimeSpan.Zero;
			BotMessages.Clear();
			UserMessages.Clear();
		}

		public Task CompletionTask() => _lifetime.Token.WhenCanceled();
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

		public static Interaction AsInteraction(this CommandContext ctx)
		{
			return new Interaction(ctx.Message);
		}
	}
}
