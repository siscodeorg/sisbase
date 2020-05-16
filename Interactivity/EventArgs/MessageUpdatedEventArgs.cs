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
		public IReadOnlyList<DiscordUser> MentionedUsers
			=> After.MentionedUsers;
		public IReadOnlyList<DiscordRole> MentionedRoles
			=> After.MentionedRoles;
		public IReadOnlyList<DiscordChannel> MentionedChannels
			=> After.MentionedChannels;
		internal MessageUpdatedEventArgs(DiscordClient client) : base (client){ }
	}
}
