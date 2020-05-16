using DSharpPlus;
using sisbase.Interactivity.Enums;
using sisbase.Interactivity.EventArgs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Interactivity {
	public class InteractionMessageListProxy : IReadOnlyList<InteractionMessage> {
		
		internal Interaction Parent;
		public List<InteractionMessage> Get()
			=> Mode == InteractionMessageListProxyMode.BOT ? Parent.BotMessages : Parent.UserMessages;
		public InteractionMessageListProxyMode Mode { get; internal set; }
		internal InteractionMessageListProxy(InteractionMessageListProxyMode mode, Interaction parent) {
            Mode = mode;
            Parent = parent;
            _reactionAdded = new AsyncEvent<ReactionAddedEventArgs>(IMC.HandleExceptions, $"{Mode}_MESSAGE_REACTION_ADDED");
            _reactionRemoved = new AsyncEvent<ReactionRemovedEventArgs>(IMC.HandleExceptions, $"{Mode}_MESSAGE_REACTION_REMOVED");
            _reactionToggled = new AsyncEvent<ReactionToggledEventArgs>(IMC.HandleExceptions, $"{Mode}_MESSAGE_REACTION_TOGGLED");
            _messageUpdated = new AsyncEvent<MessageUpdatedEventArgs>(IMC.HandleExceptions, $"{Mode}_MESSAGE_UPDATED");
            _messageDeleted = new AsyncEvent<MessageDeletedEventArgs>(IMC.HandleExceptions, $"{Mode}_MESSAGE_DELETED");
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

        internal async Task Wants(MessageReactionAddEventArgs e) {
            var message = Get().Find(x => x.Id == e.Message.Id);
            if (message == null) return;
            var sbargs = new ReactionAddedEventArgs(e.Client) { 
                Message = message,
                User = e.User,
                Emoji = e.Emoji
            };
            var toggle = new ReactionToggledEventArgs(e.Client) {
                Message = message,
                User = e.User,
                Emoji = e.Emoji,
                State = ToggleState.ADDED
            };

            await Dispatch(sbargs);
            await Dispatch(toggle);
            await First.Wants(e);
            await Last.Wants(e);

        }
        internal async Task Wants(MessageReactionRemoveEventArgs e) {
            var message = Get().Find(x => x.Id == e.Message.Id);
            if (message == null) return;
            var sbargs = new ReactionRemovedEventArgs(e.Client) {
                Message = message,
                User = e.User,
                Emoji = e.Emoji
            };
            var toggle = new ReactionToggledEventArgs(e.Client) {
                Message = message,
                User = e.User,
                Emoji = e.Emoji,
                State = ToggleState.REMOVED
            };

            await Dispatch(sbargs);
            await Dispatch(toggle);
            await First.Wants(e);
            await Last.Wants(e);
        }
        internal async Task Wants(MessageDeleteEventArgs e) {
            var message = Get().Find(x => x.Id == e.Message.Id);
            if (message == null) return;
            var sbargs = new MessageDeletedEventArgs(e.Client) {
                Message = message
            };
   
            await Dispatch(sbargs);
            await First.Wants(e);
            await Last.Wants(e);
        }
        internal async Task Wants(MessageUpdateEventArgs e) {
            var message = Get().Find(x => x.Id == e.Message.Id);
            if (message == null) return;
            var past = new PastInteractionMessage(e.MessageBefore);
            var sbargs = new MessageUpdatedEventArgs(e.Client) {
                After = message,
                Before = past
            };
            await Dispatch(sbargs);
            await First.Wants(e);
            await Last.Wants(e);
        }
        #endregion
        #region IReadOnlyList<T> Implementations
        public int Count => Mode == InteractionMessageListProxyMode.BOT ? Parent._botMessages.Count : Parent._userMessages.Count;
        public InteractionMessage this[int index] => Mode == InteractionMessageListProxyMode.BOT ? Parent._botMessages[index] : Parent._userMessages[index];
        public IEnumerator<InteractionMessage> GetEnumerator() => Mode == InteractionMessageListProxyMode.BOT ? Parent._botMessages.GetEnumerator() : Parent._userMessages.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Mode == InteractionMessageListProxyMode.BOT ? Parent._botMessages.GetEnumerator() : Parent._userMessages.GetEnumerator();
        #endregion
    }
}
