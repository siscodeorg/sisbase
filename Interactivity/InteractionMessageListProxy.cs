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
        #endregion
        #region IReadOnlyList<T> Implementations
        public int Count => Mode == InteractionMessageListProxyMode.BOT ? Parent.BotMessages.Count : Parent.UserMessages.Count;
		public InteractionMessage this[int index] => Mode == InteractionMessageListProxyMode.BOT ? Parent.BotMessages[index] : Parent.UserMessages[index];	
		public IEnumerator<InteractionMessage> GetEnumerator() => Mode == InteractionMessageListProxyMode.BOT ? Parent.BotMessages.GetEnumerator() : Parent.UserMessages.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => Mode == InteractionMessageListProxyMode.BOT ? Parent.BotMessages.GetEnumerator() : Parent.UserMessages.GetEnumerator();
		#endregion
	}
}
