using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using sisbase.Interactivity.Enums;

namespace sisbase.Interactivity.EventArgs {
	public class ReactionToggledEventArgs : DiscordEventArgs {
		public DiscordEmoji Emoji { get; internal set; }
		public ToggleState State { get; internal set; }
		public DiscordUser User { get; internal set; }
		public DiscordChannel Channel
			=> Message._Message.Channel;
		public InteractionMessage Message { get; internal set; }
		internal ReactionToggledEventArgs(DiscordClient client) : base(client) { }
	}
}
