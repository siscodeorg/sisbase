﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using sisbase.Interactivity.Enums;
using sisbase.Interactivity.EventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace sisbase.Interactivity {
    public class InteractionMessageProxy {
        public InteractionMessageProxyMode Mode { get; internal set; }
        internal InteractionMessageListProxy Parent;
        public InteractionMessage Get()
            => Mode == InteractionMessageProxyMode.FIRST ? Parent.Get().FirstOrDefault() : Parent.Get().LastOrDefault();

        internal InteractionMessageProxy(InteractionMessageProxyMode mode, InteractionMessageListProxy parent) {
            Mode = mode;
            Parent = parent;
            _reactionAdded = new AsyncEvent<ReactionAddedEventArgs>(IMC.HandleExceptions, $"{Mode}_{Parent.Mode}_MESSAGE_REACTION_ADDED");
            _reactionRemoved = new AsyncEvent<ReactionRemovedEventArgs>(IMC.HandleExceptions, $"{Mode}_{Parent.Mode}_MESSAGE_REACTION_REMOVED");
            _reactionToggled = new AsyncEvent<ReactionToggledEventArgs>(IMC.HandleExceptions, $"{Mode}_{Parent.Mode}_MESSAGE_REACTION_TOGGLED");
            _messageUpdated = new AsyncEvent<MessageUpdatedEventArgs>(IMC.HandleExceptions, $"{Mode}_{Parent.Mode}_MESSAGE_UPDATED");
            _messageDeleted = new AsyncEvent<MessageDeletedEventArgs>(IMC.HandleExceptions, $"{Mode}_{Parent.Mode}_MESSAGE_DELETED");
        }
        #region Events
        private readonly AsyncEvent<ReactionToggledEventArgs> _reactionToggled;
        private readonly AsyncEvent<ReactionAddedEventArgs> _reactionAdded;
        private readonly AsyncEvent<ReactionRemovedEventArgs> _reactionRemoved;
        private readonly AsyncEvent<MessageUpdatedEventArgs> _messageUpdated;
        private readonly AsyncEvent<MessageDeletedEventArgs> _messageDeleted;

        public event AsyncEventHandler<ReactionToggledEventArgs> ReactionToggled {
            add => _reactionToggled.Register(value);
            remove => _reactionToggled.Unregister(value);
        }
        public event AsyncEventHandler<ReactionAddedEventArgs> ReactionAdded {
            add => _reactionAdded.Register(value);
            remove => _reactionAdded.Register(value);
        }
        public event AsyncEventHandler<ReactionRemovedEventArgs> ReactionRemoved {
            add => _reactionRemoved.Register(value);
            remove => _reactionRemoved.Unregister(value);
        }
        public event AsyncEventHandler<MessageUpdatedEventArgs> MessageUpdated {
            add => _messageUpdated.Register(value);
            remove => _messageUpdated.Unregister(value);
        }
        public event AsyncEventHandler<MessageDeletedEventArgs> MessageDeleted {
            add => _messageDeleted.Register(value);
            remove => _messageDeleted.Unregister(value);
        }

        private async Task Dispatch(ReactionAddedEventArgs e) => await _reactionAdded.InvokeAsync(e);
        private async Task Dispatch(ReactionToggledEventArgs e) => await _reactionToggled.InvokeAsync(e);
        private async Task Dispatch(ReactionRemovedEventArgs e) => await _reactionRemoved.InvokeAsync(e);
        private async Task Dispatch(MessageDeletedEventArgs e) => await _messageDeleted.InvokeAsync(e);
        private async Task Dispatch(MessageUpdatedEventArgs e) => await _messageUpdated.InvokeAsync(e);

        internal async Task Offer(MessageReactionAddEventArgs e) {
            if (e.Message.Id == Get().Id) {
                var sbargs = new ReactionAddedEventArgs(e.Client) {
                    Message = Get(),
                    User = e.User,
                    Emoji = e.Emoji
                };
                var toggle = new ReactionToggledEventArgs(e.Client) {
                    Message = Get(),
                    User = e.User,
                    Emoji = e.Emoji,
                    State = ToggleState.ADDED
                };
                await Dispatch(sbargs);
                await Dispatch(toggle);
            }

        }
        internal async Task Offer(MessageReactionRemoveEventArgs e) {
            if (e.Message.Id == Get().Id) {
                var sbargs = new ReactionRemovedEventArgs(e.Client) {
                    Message = Get(),
                    User = e.User,
                    Emoji = e.Emoji
                };
                var toggle = new ReactionToggledEventArgs(e.Client) {
                    Message = Get(),
                    User = e.User,
                    Emoji = e.Emoji,
                    State = ToggleState.REMOVED
                };
                await Dispatch(sbargs);
                await Dispatch(toggle);
            }
        }
        internal async Task Offer(MessageDeleteEventArgs e) {
            if (e.Message.Id == Get().Id) {
                var sbargs = new MessageDeletedEventArgs(e.Client) {
                    Message = Get()
                };
                await Dispatch(sbargs);
            }

        }
        internal async Task Offer(MessageUpdateEventArgs e) {
            if (e.Message.Id == Get().Id) {
                var past = new PastInteractionMessage(e.MessageBefore);
                var sbargs = new MessageUpdatedEventArgs(e.Client) {
                    After = Get(),
                    Before = past
                };
                await Dispatch(sbargs);
            }
        }
        #endregion
        #region Event Waiters

        public async Task<ReactionAddedEventArgs> WaitReactionAdded(Func<ReactionAddedEventArgs, bool> pred, TimeSpan timeout = default, CancellationToken token = default) {
            var waiter = new EventWaiter<MessageReactionAddEventArgs>(e => {
                var msg = Get();
                if (e.Message.Id != msg.Id) return false;
                return pred(new ReactionAddedEventArgs(e, msg));
            }, timeout, token);
            IMC.GetInteractivityManager().ReactionAddWaiter.Register(waiter);
            var args = await waiter.Task;
            return new ReactionAddedEventArgs(args, Get());
        }
        
        public async Task<ReactionRemovedEventArgs> WaitReactionRemoved(Func<ReactionRemovedEventArgs, bool> pred, TimeSpan timeout = default, CancellationToken token = default) {
			var waiter = new EventWaiter<MessageReactionRemoveEventArgs>(e => {
                var msg = Get();
                if (e.Message.Id != msg.Id) return false;
				return pred(new ReactionRemovedEventArgs(e, msg));
			}, timeout, token);
			IMC.GetInteractivityManager().ReactionRemoveWaiter.Register(waiter);
			var args = await waiter.Task;
			return new ReactionRemovedEventArgs(args, Get());
		}
		
		public async Task<ReactionToggledEventArgs> WaitReactionToggled(Func<ReactionToggledEventArgs, bool> pred, TimeSpan timeout = default, CancellationToken token = default) {
			var waiter = new EventWaiter<DiscordEventArgs>(e => {
				if (e is MessageReactionAddEventArgs added) {
                    var msg = Get();
                    if (added.Message.Id != msg.Id) return false;
					return pred(new ReactionToggledEventArgs(added, msg));
				}
				if (e is MessageReactionRemoveEventArgs removed) {
                    var msg = Get();
                    if (removed.Message.Id != msg.Id) return false;
					return pred(new ReactionToggledEventArgs(removed, msg));
				}

				return false;
			}, timeout, token);
			IMC.GetInteractivityManager().ReactionToggleWaiter.Register(waiter);
			var args = await waiter.Task;
			if (args is MessageReactionAddEventArgs added) {
				return new ReactionToggledEventArgs(added, Get());
			}
			if (args is MessageReactionRemoveEventArgs removed) {
				return new ReactionToggledEventArgs(removed, Get());
			}
			throw new Exception("ReactionToggled waiter returned a non-reaction event. Please report this to the sisbase devs");
		}
		
		public async Task<MessageDeletedEventArgs> WaitMessageDeleted(Func<MessageDeletedEventArgs, bool> pred, TimeSpan timeout = default, CancellationToken token = default) {
			var waiter = new EventWaiter<MessageDeleteEventArgs>(e => {
                var msg = Get();
                if (e.Message.Id != msg.Id) return false;
				return pred(new MessageDeletedEventArgs(e, msg));
			}, timeout, token);
			IMC.GetInteractivityManager().MessageDeleteWaiter.Register(waiter);
			var args = await waiter.Task;
			return new MessageDeletedEventArgs(args, Get());
		}
		
		public async Task<MessageUpdatedEventArgs> WaitMessageEdited(Func<MessageUpdatedEventArgs, bool> pred, TimeSpan timeout = default, CancellationToken token = default) {
			var waiter = new EventWaiter<MessageUpdateEventArgs>(e => {
                var msg = Get();
                if (e.Message.Id != msg.Id) return false;
				return pred(new MessageUpdatedEventArgs(e, msg));
			}, timeout, token);
			IMC.GetInteractivityManager().EditWaiter.Register(waiter);
			var args = await waiter.Task;
			return new MessageUpdatedEventArgs(args, Get());
		}

        #endregion
        #region D#+ Delegation
        public DiscordChannel Channel => Get().Channel;
        public ulong ChannelId => Get().ChannelId;
        public DiscordUser Author => Get().Author;
        public string Content => Get().Content;
        public DateTimeOffset Timestamp => Get().Timestamp;
        public DateTimeOffset? EditedTimestamp => Get().EditedTimestamp;
        public bool IsEdited => Get().IsEdited;
        public bool IsTTS => Get().IsTTS;
        public bool MentionEveryone => Get().MentionEveryone;
        public IReadOnlyList<DiscordUser> MentionedUsers => Get().MentionedUsers;
        public IReadOnlyList<DiscordRole> MentionedRoles => Get().MentionedRoles;
        public IReadOnlyList<DiscordChannel> MentionedChannels => Get().MentionedChannels;
        public IReadOnlyList<DiscordAttachment> Attachments => Get().Attachments;
        public IReadOnlyList<DiscordEmbed> Embeds => Get().Embeds;
        public IReadOnlyList<DiscordReaction> Reactions => Get().Reactions;
        public bool Pinned => Get().Pinned;
        public ulong? WebhookId => Get().WebhookId;
        public MessageType? MessageType => Get().MessageType;
        public DiscordMessageActivity Activity => Get().Activity;
        public DiscordMessageApplication Application => Get().Application;
        public DiscordMessageReference Reference => Get().Reference;
        public MessageFlags? Flags => Get().Flags;
        public bool WebhookMessage => Get().WebhookMessage;
        public Uri JumpLink => Get().JumpLink;

        public Task<InteractionMessage> ModifyAsync(Optional<string> content = default,
            Optional<DiscordEmbed> embed = default) => Get().ModifyAsync(content, embed);

        public Task DeleteAsync(string reason = null) => Get().DeleteAsync(reason);
        public Task ModifyEmbedSuppressionAsync(bool hideEmbeds) => Get().ModifyEmbedSuppressionAsync(hideEmbeds);
        public Task PinAsync() => Get().PinAsync();
        public Task UnpinAsync() => Get().UnpinAsync();

        public Task<InteractionMessage> RespondAsync(string content = null, bool tts = false, DiscordEmbed embed = null,
            IEnumerable<IMention> mentions = null) => Get().RespondAsync(content, tts, embed, mentions);

        public Task<InteractionMessage> RespondWithFileAsync(string fileName, Stream fileData, string content = null,
            bool tts = false, DiscordEmbed embed = null, IEnumerable<IMention> mentions = null) =>
            Get().RespondWithFileAsync(fileName, fileData, content, tts, embed, mentions);

        public Task<InteractionMessage> RespondWithFileAsync(FileStream fileData, string content = null,
            bool tts = false, DiscordEmbed embed = null, IEnumerable<IMention> mentions = null) =>
            Get().RespondWithFileAsync(fileData, content, tts, embed, mentions);

        public Task<InteractionMessage> RespondWithFileAsync(string filePath, string content = null,
            bool tts = false, DiscordEmbed embed = null, IEnumerable<IMention> mentions = null) =>
            Get().RespondWithFileAsync(filePath, content, tts, embed, mentions);

        public Task<InteractionMessage> RespondWithFilesAsync(Dictionary<string, Stream> files, string content = null,
            bool tts = false, DiscordEmbed embed = null, IEnumerable<IMention> mentions = null) =>
            Get().RespondWithFilesAsync(files, content, tts, embed, mentions);

        public Task CreateReactionAsync(DiscordEmoji emoji) => Get().CreateReactionAsync(emoji);
        public Task DeleteOwnReactionAsync(DiscordEmoji emoji) => Get().DeleteOwnReactionAsync(emoji);

        public Task DeleteReactionAsync(DiscordEmoji emoji, DiscordUser user, string reason = null) =>
            Get().DeleteReactionAsync(emoji, user, reason);

        public Task<IReadOnlyList<DiscordUser>> GetReactionsAsync(DiscordEmoji emoji, int limit = 25,
            ulong? after = null) => Get().GetReactionsAsync(emoji, limit, after);

        public Task DeleteAllReactionsAsync(string reason = null) => Get().DeleteAllReactionsAsync(reason);
        public Task DeleteReactionsEmojiAsync(DiscordEmoji emoji) => Get().DeleteReactionsEmojiAsync(emoji);
        public override string ToString() => Get().ToString();
        #endregion

    }
}