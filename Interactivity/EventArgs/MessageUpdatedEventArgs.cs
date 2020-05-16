using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;

namespace sisbase.Interactivity.EventArgs
{
	public class MessageUpdatedEventArgs : DiscordEventArgs
	{
		public InteractionMessage After { get; internal set; }
		public PastInteractionMessage Before { get; internal set; }
		public DiscordChannel Channel
			=> After.Channel;
		public DiscordGuild Guild
			=> Channel.Guild;
		public DiscordUser Author
			=> After.Author;
		public IReadOnlyList<DiscordUser> MentionedUsers { get; internal set; }
		public IReadOnlyList<DiscordRole> MentionedRoles { get; internal set; }
		public IReadOnlyList<DiscordChannel> MentionedChannels { get; internal set; }
		internal MessageUpdatedEventArgs(DiscordClient client) : base (client){ }
	}
}
