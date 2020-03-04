using DSharpPlus.Entities;
using System;

namespace sisbase.Attributes
{
	public class EmojiAttribute : Attribute
	{
		public DiscordEmoji Emoji;

		public EmojiAttribute(DiscordEmoji emoji) => Emoji = emoji;

		public EmojiAttribute(string unicode) => Emoji = DiscordEmoji.FromUnicode(Program.Client, unicode) ?? DiscordEmoji.FromName(Program.Client, unicode);

		public EmojiAttribute(ulong id) => Emoji = DiscordEmoji.FromGuildEmote(Program.Client, id);
	}
}