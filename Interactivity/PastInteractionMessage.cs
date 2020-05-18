using DSharpPlus.Entities;
using System;
using System.Collections.Generic;

namespace sisbase.Interactivity {
	public class PastInteractionMessage {
		public string Content { get; internal set; }
		public IReadOnlyList<DiscordEmbed> Embeds { get; internal set; }
		public IReadOnlyList<DiscordUser> MentionedUsers { get; internal set; }
		public IReadOnlyList<DiscordRole> MentionedRoles { get; internal set; }
		public IReadOnlyList<DiscordChannel> MentionedChannels { get; internal set; }
		public DateTimeOffset Timestamp { get; internal set; }
		public DateTimeOffset? EditedTimestamp { get; internal set; }

		internal PastInteractionMessage(DiscordMessage m) {
			Content = m.Content;
			Embeds = m.Embeds;
			MentionedChannels = m.MentionedChannels;
			MentionedRoles = m.MentionedRoles;
			MentionedUsers = m.MentionedUsers;
			Timestamp = m.Timestamp;
			EditedTimestamp = m.EditedTimestamp;
		}
	}
}
