using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace sisbase.Interactivity
{
	public static class EventArgs
	{
		//OnLastMessageReact,OnOriginMessageReact,OnMessageReact
		//|:MessageEdit , MessageDelete
		public class MessageReactArgs : System.EventArgs
		{
			public DiscordMessage Message { get; }
			public DiscordEmoji Emoji { get; }
			public DiscordUser User { get; }

			public MessageReactArgs(DiscordMessage message, DiscordEmoji emoji, DiscordUser user)
			{
				Message = message;
				Emoji = emoji;
				User = user;
			}
		}

		public class MessageEditArgs : System.EventArgs
		{
			public DiscordMessage Before { get; }
			public DiscordMessage After { get; }

			public MessageEditArgs(DiscordMessage before, DiscordMessage after)
			{
				Before = before;
				After = after;
			}
		}

		public class MessageDeleteArgs : System.EventArgs 
		{
			public DiscordMessage Message { get; }
			public MessageDeleteArgs(DiscordMessage message)
			 => Message = message;
		}
	}
}
