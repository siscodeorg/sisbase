using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;

namespace sisbase.Interactivity.EventArgs
{
	public class MessageDeletedEventArgs : DiscordEventArgs
	{
		public InteractionMessage Message { get; internal set; }
		public DiscordChannel Channel
			=> Message.Channel;
		public DiscordUser User { get; internal set; }
		public MessageDeletedEventArgs(DiscordClient client) : base(client)
		{}
	}
}
