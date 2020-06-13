using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using sisbase.Interactivity.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace sisbase.Interactivity.EventArgs
{
	public class ReactionToggledEventArgs : DiscordEventArgs
	{
		public DiscordEmoji Emoji { get; internal set; }
		public ToggleState State { get; internal set; }
		public DiscordUser User { get; internal set; }
		public DiscordChannel Channel
			=> Message._Message.Channel;
		public InteractionMessage Message { get; internal set; }
		internal ReactionToggledEventArgs(DiscordClient client) : base(client) { }
		
		internal ReactionToggledEventArgs(MessageReactionRemoveEventArgs dspargs, InteractionMessage owner) : base(dspargs.Client) {
			Emoji = dspargs.Emoji;
			User = dspargs.User;
			State = ToggleState.REMOVED;
			Message = owner;
		}
		
		internal ReactionToggledEventArgs(MessageReactionAddEventArgs dspargs, InteractionMessage owner) : base(dspargs.Client) {
			Emoji = dspargs.Emoji;
			User = dspargs.User;
			State = ToggleState.ADDED;
			Message = owner;
		}
	}
}
