using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using sisbase.Interactivity.EventArgs;
using sisbase.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace sisbase.Interactivity
{
	public class InteractionMessage 
	{
		public DiscordMessage _Message;
		internal Interaction _Owner;
		internal List<PastInteractionMessage> _history = new List<PastInteractionMessage>();
		#region Properties
		public DiscordMessageActivity Activity => _Message.Activity;
		public DiscordMessageApplication Application => _Message.Application;
		public IReadOnlyList<DiscordAttachment> Attachments => _Message.Attachments;
		public IReadOnlyList<PastInteractionMessage> History => _history;
		public DiscordUser Author => _Message.Author;
		public DiscordChannel Channel => _Message.Channel;
		public ulong ChannelId => _Message.ChannelId;
		public string Content => _Message.Content;
		public DateTimeOffset? EditedTimestamp => _Message.EditedTimestamp;
		public IReadOnlyList<DiscordEmbed> Embeds => _Message.Embeds;
		public MessageFlags? Flags => _Message.Flags;
		public bool IsEdited => _Message.IsEdited;
		public bool IsTTS => _Message.IsTTS;
		public Uri JumpLink => _Message.JumpLink;
		public IReadOnlyList<DiscordChannel> MentionedChannels => _Message.MentionedChannels;
		public IReadOnlyList<DiscordRole> MentionedRoles => _Message.MentionedRoles;
		public IReadOnlyList<DiscordUser> MentionedUsers => _Message.MentionedUsers;
		public bool MentionEveryone => _Message.MentionEveryone;
		public MessageType? MessageType => _Message.MessageType;
		public bool Pinned => _Message.Pinned;
		public IReadOnlyList<DiscordReaction> Reactions => _Message.Reactions;
		public DiscordMessageReference Reference => _Message.Reference;
		public DateTimeOffset Timestamp => _Message.Timestamp;
		public ulong? WebhookId => _Message.WebhookId;
		public bool WebhookMessage => _Message.WebhookMessage;
		public ulong Id => _Message.Id;
		#endregion
		#region Delegates
		private AsyncEvent<ReactionAddedEventArgs> _reactionAdded;
		private AsyncEvent<ReactionRemovedEventArgs> _reactionRemoved;
		private AsyncEvent<ReactionToggledEventArgs> _reactionToggled;
		private AsyncEvent<MessageDeletedEventArgs> _messageDeleted;
		private AsyncEvent<MessageUpdatedEventArgs> _messageUpdated;
		#endregion
		#region Events
		public event AsyncEventHandler<ReactionAddedEventArgs> ReactionAdded
		{
			add => _reactionAdded.Register(value);
			remove => _reactionAdded.Unregister(value);
		}
		public event AsyncEventHandler<ReactionRemovedEventArgs> ReactionRemoved
		{
			add => _reactionRemoved.Register(value);
			remove => _reactionRemoved.Register(value);
		}
		public event AsyncEventHandler<ReactionToggledEventArgs> ReactionToggled
		{
			add => _reactionToggled.Register(value);
			remove => _reactionToggled.Unregister(value);
		}
		public event AsyncEventHandler<MessageDeletedEventArgs> MessageDeleted
		{
			add => _messageDeleted.Register(value);
			remove => _messageDeleted.Unregister(value);
		}
		public event AsyncEventHandler<MessageUpdatedEventArgs> MessageUpdated
		{
			add => _messageUpdated.Register(value);
			remove => _messageUpdated.Unregister(value);
		}
		#endregion
		#region Event Dispatchers
		private async Task Dispatch(ReactionAddedEventArgs e)
			=> await _reactionAdded.InvokeAsync(e);
		private async Task Dispatch(ReactionRemovedEventArgs e)
			=> await _reactionRemoved.InvokeAsync(e);
		private async Task Dispatch(ReactionToggledEventArgs e)
			=> await _reactionToggled.InvokeAsync(e);
		private async Task Dispatch(MessageUpdatedEventArgs e)
			=> await _messageUpdated.InvokeAsync(e);
		private async Task Dispatch(MessageDeletedEventArgs e)
			=> await _messageDeleted.InvokeAsync(e);
		internal async Task Wants(MessageReactionAddEventArgs e) {
			if (e.Message.Id == Id) {
				var sbargs = new ReactionAddedEventArgs(e.Client) {
					Emoji = e.Emoji,
					Message = this,
					User = e.User
				};
				var toggle = new ReactionToggledEventArgs(e.Client) {
					Emoji = e.Emoji,
					Message = this,
					User = e.User,
					State = Enums.ToggleState.ADDED
				};
				await Dispatch(toggle);
				await Dispatch(sbargs);
			}
				
		}
		internal async Task Wants(MessageReactionRemoveEventArgs e) {
			if (e.Message.Id == Id) {
				var sbargs = new ReactionRemovedEventArgs(e.Client) {
					Emoji = e.Emoji,
					Message = this,
					User = e.User
				};
				var toggle = new ReactionToggledEventArgs(e.Client) {
					Emoji = e.Emoji,
					Message = this,
					User = e.User,
					State = Enums.ToggleState.REMOVED
				};
				await Dispatch(toggle);
				await Dispatch(sbargs);
			}
		}
		internal async Task Wants(MessageDeleteEventArgs e) {
			if (e.Message.Id == Id) {
				var sbargs = new MessageDeletedEventArgs(e.Client) {
					Message = this
				};
				await Dispatch(sbargs);
			}
		}
		internal async Task Wants(MessageUpdateEventArgs e) {
			if (e.MessageBefore.Id == Id) {
				var past = new PastInteractionMessage(e.MessageBefore);
				if(!History.Contains(past)) _history.Add(past);
				var sbargs = new MessageUpdatedEventArgs(e.Client) {
					After = this,
					Before = past
				};
				await Dispatch(sbargs);
			}
				
		}

		#endregion
		public Task DeleteAsync(string reason = "")
			=> _Owner.RemoveAsync(this, reason);

		public async Task MutateAsync(Action<MessageBuilder> action)
		{
			var builder = new MessageBuilder(_Message);
			action(builder);
			_Message = await builder.Build(_Message.Channel);
		}
		public Task RespondAsync(MessageBuilder message) 
			=>  _Owner.SendMessageAsync(message);
		public async Task ToggleReactionAsync(DiscordEmoji reaction)
		{
			if ((await _Message.GetReactionsAsync(reaction)).Contains(SisbaseBot.Instance.Client.CurrentUser))
				await _Message.DeleteOwnReactionAsync(reaction);
			else
				await _Message.CreateReactionAsync(reaction);
		}
		public Task CreateReactionAsync(DiscordEmoji emoji) 
			=> _Message.CreateReactionAsync(emoji);
		public Task DeleteOwnReactionAsync(DiscordEmoji emoji) 
			=> _Message.DeleteOwnReactionAsync(emoji);
		public Task ModifyEmbedSuppressionAsync(bool hideEmbeds) 
			=> _Message.ModifyEmbedSuppressionAsync(hideEmbeds);
		public async Task<InteractionMessage> ModifyAsync(Optional<string> content = default,
				Optional<DiscordEmbed> embed = default)
		{ await _Message.ModifyAsync(content, embed); return this; }
		public Task<InteractionMessage> RespondAsync(string content = null, bool tts = false, DiscordEmbed embed = null,
				IEnumerable<IMention> mentions = null) 
			=>  _Owner.SendMessageAsync(new MessageBuilder()
				.WithContent(content)
				.WithEmbed(embed)
				.WithMentions(mentions.ToList())
				.SetTTS(tts));
		public Task<InteractionMessage> RespondWithFileAsync(string fileName, Stream fileData, string content = null,
			   bool tts = false, DiscordEmbed embed = null, IEnumerable<IMention> mentions = null)
			=> _Owner.SendMessageAsync(new MessageBuilder()
				.WithContent(content)
				.WithEmbed(embed)
				.WithMentions(mentions as List<IMention>)
				.SetTTS(tts)
				.Bind(fileData,fileName));
		public Task<InteractionMessage> RespondWithFileAsync(FileStream fileData, string content = null,
				bool tts = false, DiscordEmbed embed = null, IEnumerable<IMention> mentions = null) 
			=> _Owner.SendMessageAsync(new MessageBuilder()
				.WithContent(content)
				.WithEmbed(embed)
				.WithMentions(mentions as List<IMention>)
				.SetTTS(tts)
				.Bind(fileData));
		public Task<InteractionMessage> RespondWithFileAsync(string filePath, string content = null,
			   bool tts = false, DiscordEmbed embed = null, IEnumerable<IMention> mentions = null) 
			=> _Owner.SendMessageAsync(new MessageBuilder()
				.WithContent(content)
				.WithEmbed(embed)
				.WithMentions(mentions as List<IMention>)
				.SetTTS(tts)
				.Bind(filePath));
		public Task<InteractionMessage> RespondWithFilesAsync(Dictionary<string, Stream> files, string content = null,
				bool tts = false, DiscordEmbed embed = null, IEnumerable<IMention> mentions = null)
			=> _Owner.SendMessageAsync(new MessageBuilder()
				.WithContent(content)
				.WithEmbed(embed)
				.WithMentions(mentions as List<IMention>)
				.SetTTS(tts)
				.BindMany(files));
		public Task<IReadOnlyList<DiscordUser>> GetReactionsAsync(DiscordEmoji emoji, int limit = 25,
				ulong? after = null)
			=> _Message.GetReactionsAsync(emoji, limit, after);
		public Task DeleteAllReactionsAsync(string reason = null)
			=> _Message.DeleteAllReactionsAsync(reason);
		public Task DeleteReactionsEmojiAsync(DiscordEmoji emoji)
			=> _Message.DeleteReactionsEmojiAsync(emoji);

		public Task PinAsync() 
			=> _Message.PinAsync();
		public Task UnpinAsync()
			=> _Message.UnpinAsync();
		public Task DeleteReactionAsync(DiscordEmoji emoji, DiscordUser user, string reason = null) =>
			_Message.DeleteReactionAsync(emoji, user, reason);
		#region Constr
		public InteractionMessage(DiscordMessage m, Interaction i)
		{
			_Message = m;
			_Owner = i;
			_messageDeleted = new AsyncEvent<MessageDeletedEventArgs>(IMC.HandleExceptions, "SISBASE_MESSAGE_DELETED_EVENT");
			_messageUpdated = new AsyncEvent<MessageUpdatedEventArgs>(IMC.HandleExceptions, "SISBASE_MESSSAGE_UPDATED_EVENT");
			_reactionAdded = new AsyncEvent<ReactionAddedEventArgs>(IMC.HandleExceptions, "SISBASE_REACTION_ADDED_EVENT");
			_reactionRemoved = new AsyncEvent<ReactionRemovedEventArgs>(IMC.HandleExceptions, "SISBASE_REACTION_REMOVED_EVENT");
			_reactionToggled = new AsyncEvent<ReactionToggledEventArgs>(IMC.HandleExceptions, "SISBASE_REACTION_TOGGLED_EVENT");
		}
		#endregion
	}
}
