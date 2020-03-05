using DSharpPlus.Entities;
using System;

namespace sisbase.Attributes
{
	public class EmojiAttribute : Attribute
	{
		public DiscordEmoji Emoji;

		public EmojiAttribute(DiscordEmoji emoji) => Emoji = emoji;

		public EmojiAttribute(string unicode) => Emoji = DiscordEmoji.FromUnicode(SisbaseBot.Instance.Client, unicode) ?? DiscordEmoji.FromName(SisbaseBot.Instance.Client, unicode);

		public EmojiAttribute(ulong id) => Emoji = DiscordEmoji.FromGuildEmote(SisbaseBot.Instance.Client, id);
	}
}