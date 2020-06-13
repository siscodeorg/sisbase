using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace sisbase.Interactivity.EventArgs
{
	public class ReactionRemovedEventArgs : DiscordEventArgs
	{
		public DiscordEmoji Emoji { get; internal set; }
		public DiscordUser User { get; internal set; }
		public InteractionMessage Message { get; internal set; }
		public DiscordChannel Channel
			=> Message.Channel;

		internal ReactionRemovedEventArgs(DiscordClient client) : base(client) { }

		internal ReactionRemovedEventArgs(MessageReactionRemoveEventArgs dspargs, InteractionMessage owner) : base(dspargs.Client)
		{
			Emoji = dspargs.Emoji;
			User = dspargs.User;
			Message = owner;
		}
	}
}
