using System;
using DSharpPlus;
using DSharpPlus.EventArgs;
using sisbase.Interactivity.Enums;
using sisbase.Interactivity.EventArgs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace sisbase.Interactivity {
    public class InteractionMessageListProxy : IReadOnlyList<InteractionMessage> {

        internal Interaction Parent;
        public List<InteractionMessage> Get()
            => Mode == InteractionMessageListProxyMode.BOT ? Parent._botMessages : Parent._userMessages;
        public InteractionMessageListProxyMode Mode { get; internal set; }
        public InteractionMessageProxy First { get; internal set; }
        public InteractionMessageProxy Last { get; internal set; }
        internal InteractionMessageListProxy(InteractionMessageListProxyMode mode, Interaction parent) {
            Mode = mode;
            Parent = parent;
            _reactionAdded = new AsyncEvent<ReactionAddedEventArgs>(IMC.HandleExceptions, $"{Mode}_MESSAGE_REACTION_ADDED");
            _reactionRemoved = new AsyncEvent<ReactionRemovedEventArgs>(IMC.HandleExceptions, $"{Mode}_MESSAGE_REACTION_REMOVED");
            _reactionToggled = new AsyncEvent<ReactionToggledEventArgs>(IMC.HandleExceptions, $"{Mode}_MESSAGE_REACTION_TOGGLED");
            _messageUpdated = new AsyncEvent<MessageUpdatedEventArgs>(IMC.HandleExceptions, $"{Mode}_MESSAGE_UPDATED");
            _messageDeleted = new AsyncEvent<MessageDeletedEventArgs>(IMC.HandleExceptions, $"{Mode}_MESSAGE_DELETED");
            First = new InteractionMessageProxy(InteractionMessageProxyMode.FIRST, this);
            Last = new InteractionMessageProxy(InteractionMessageProxyMode.LAST, this);
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
            await First.Offer(e);
            await Last.Offer(e);

        }
        internal async Task Offer(MessageReactionRemoveEventArgs e) {
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
            await First.Offer(e);
            await Last.Offer(e);
        }
        internal async Task Offer(MessageDeleteEventArgs e) {
            var message = Get().Find(x => x.Id == e.Message.Id);
            if (message == null) return;
            var sbargs = new MessageDeletedEventArgs(e.Client) {
                Message = message
            };
   
            await Dispatch(sbargs);
            await First.Offer(e);
            await Last.Offer(e);
        }
        internal async Task Offer(MessageUpdateEventArgs e) {
            var message = Get().Find(x => x.Id == e.Message.Id);
            if (message == null) return;
            var past = new PastInteractionMessage(e.MessageBefore);
            var sbargs = new MessageUpdatedEventArgs(e.Client) {
                After = message,
                Before = past
            };
            await Dispatch(sbargs);
            await First.Offer(e);
            await Last.Offer(e);
        }
        #endregion
        #region Awaitables

        public async Task<ReactionAddedEventArgs> WaitReactionAdded(Func<ReactionAddedEventArgs, bool> pred, TimeSpan timeout = default, CancellationToken token = default) {
            var waiter = new EventWaiter<MessageReactionAddEventArgs>(args => { return Get()
                .Where(msg => msg.Id == args.Message.Id)
                .Any(msg => pred(new ReactionAddedEventArgs(args, msg))); 
            });
            IMC.GetInteractivityManager().ReactionAddWaiter.Register(waiter);
            var dspargs = await waiter.Task;
            foreach (var msg in Get().Where(msg => msg.Id == dspargs.Message.Id)) {
                return new ReactionAddedEventArgs(dspargs, msg);
            }

            throw new Exception("A InteractionMessageListProxy waiter matched an irrelevant message. please report this to the sisbase devs");
        }
        
        public async Task<ReactionRemovedEventArgs> WaitReactionRemoved(Func<ReactionRemovedEventArgs, bool> pred, TimeSpan timeout = default, CancellationToken token = default) {
            var waiter = new EventWaiter<MessageReactionRemoveEventArgs>(args => { return Get()
                .Where(msg => msg.Id == args.Message.Id)
                .Any(msg => pred(new ReactionRemovedEventArgs(args, msg))); 
            });
            IMC.GetInteractivityManager().ReactionRemoveWaiter.Register(waiter);
            var dspargs = await waiter.Task;
            foreach (var msg in Get().Where(msg => msg.Id == dspargs.Message.Id)) {
                return new ReactionRemovedEventArgs(dspargs, msg);
            }

            throw new Exception("A InteractionMessageListProxy waiter matched an irrelevant message. please report this to the sisbase devs");
        }
        
        public async Task<ReactionToggledEventArgs> WaitReactionToggled(Func<ReactionToggledEventArgs, bool> pred, TimeSpan timeout = default, CancellationToken token = default) {
            var waiter = new EventWaiter<DiscordEventArgs>(args => {
                if (args is MessageReactionAddEventArgs added) {
                    return Get()
                        .Where(msg => msg.Id == added.Message.Id)
                        .Any(msg => pred(new ReactionToggledEventArgs(added, msg)));
                }
                if (args is MessageReactionRemoveEventArgs removed) {
                    return Get()
                        .Where(msg => msg.Id == removed.Message.Id)
                        .Any(msg => pred(new ReactionToggledEventArgs(removed, msg)));
                }

                return false;
            });
            IMC.GetInteractivityManager().ReactionToggleWaiter.Register(waiter);
            var dspargs = await waiter.Task;
            if (dspargs is MessageReactionAddEventArgs added) {
                foreach (var msg in Get().Where(msg => msg.Id == added.Message.Id)) {
                    return new ReactionToggledEventArgs(added, msg);
                }
                throw new Exception("A InteractionMessageListProxy waiter matched an irrelevant message. please report this to the sisbase devs");
            }
            if (dspargs is MessageReactionRemoveEventArgs removed) {
                foreach (var msg in Get().Where(msg => msg.Id == removed.Message.Id)) {
                    return new ReactionToggledEventArgs(removed, msg);
                }
                throw new Exception("A InteractionMessageListProxy waiter matched an irrelevant message. please report this to the sisbase devs");
            }
            throw new Exception("ReactionToggled waiter returned a non-reaction event. Please report this to the sisbase devs");
        }
        
        public async Task<MessageDeletedEventArgs> WaitMessageDeleted(Func<MessageDeletedEventArgs, bool> pred, TimeSpan timeout = default, CancellationToken token = default) {
            var waiter = new EventWaiter<MessageDeleteEventArgs>(args => { return Get()
                .Where(msg => msg.Id == args.Message.Id)
                .Any(msg => pred(new MessageDeletedEventArgs(args, msg))); 
            });
            IMC.GetInteractivityManager().MessageDeleteWaiter.Register(waiter);
            var dspargs = await waiter.Task;
            foreach (var msg in Get().Where(msg => msg.Id == dspargs.Message.Id)) {
                return new MessageDeletedEventArgs(dspargs, msg);
            }

            throw new Exception("A InteractionMessageListProxy waiter matched an irrelevant message. please report this to the sisbase devs");
        }
        
        public async Task<MessageUpdatedEventArgs> WaitMessageEdited(Func<MessageUpdatedEventArgs, bool> pred, TimeSpan timeout = default, CancellationToken token = default) {
            var waiter = new EventWaiter<MessageUpdateEventArgs>(args => { return Get()
                .Where(msg => msg.Id == args.Message.Id)
                .Any(msg => pred(new MessageUpdatedEventArgs(args, msg))); 
            });
            IMC.GetInteractivityManager().EditWaiter.Register(waiter);
            var dspargs = await waiter.Task;
            foreach (var msg in Get().Where(msg => msg.Id == dspargs.Message.Id)) {
                return new MessageUpdatedEventArgs(dspargs, msg);
            }

            throw new Exception("A InteractionMessageListProxy waiter matched an irrelevant message. please report this to the sisbase devs");
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
