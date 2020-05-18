using DSharpPlus.Entities;
using System;

namespace sisbase.Attributes {
	/// <summary>
	/// Atribute that sets the emoji for the command group
	/// </summary>
	public class EmojiAttribute : Attribute {
		/// <summary>
		/// The emoji that will be used by the command group
		/// </summary>
		public DiscordEmoji Emoji;

		/// <summary>
		/// Constructs a new EmojiAttribute from a <see cref="DiscordEmoji"/>
		/// </summary>
		/// <param name="emoji">The emoji</param>
		public EmojiAttribute(DiscordEmoji emoji) => Emoji = emoji;

		/// <summary>
		/// Constructs a new EmojiAttribute from an unicode name <br></br>
		/// Eg. :computer: , :white_check_mark:
		/// </summary>
		/// <param name="name">The discord name(requires being surrounded by colons)</param>
		public EmojiAttribute(string name) => Emoji = DiscordEmoji.FromName(SisbaseBot.Instance.Client, name);
		/// <summary>
		/// Constructs a new EmojiAttribute from an known guild emoji id.
		/// </summary>
		/// <param name="id">The id</param>
		public EmojiAttribute(ulong id) => Emoji = DiscordEmoji.FromGuildEmote(SisbaseBot.Instance.Client, id);
	}
}