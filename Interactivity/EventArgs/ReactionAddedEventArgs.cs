using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;

namespace sisbase.Interactivity.EventArgs
{
	public class ReactionAddedEventArgs : DiscordEventArgs
	{
		public DiscordEmoji Emoji { get; internal set; }
		public DiscordUser User { get; internal set;  }
		public InteractionMessage Message { get; internal set; }
		public DiscordChannel Channel
			=> Message.Channel;
		internal ReactionAddedEventArgs(DiscordClient client) : base(client)
		{}

		internal ReactionAddedEventArgs(MessageReactionAddEventArgs dspargs, InteractionMessage owner) : base(dspargs.Client) {
			Emoji = dspargs.Emoji;
			User = dspargs.User;
			Message = owner;
		}

	}
}
