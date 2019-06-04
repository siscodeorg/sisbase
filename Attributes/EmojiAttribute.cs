using System;
using DSharpPlus.Entities;

namespace LA_RPbot.Discord.Attributes
{
    public class EmojiAttribute : Attribute
    {
        public DiscordEmoji Emoji;

        public EmojiAttribute(DiscordEmoji emoji)
        {
            this.Emoji = emoji;
        }
        
        public EmojiAttribute(string unicode)
        {
            this.Emoji = DiscordEmoji.FromUnicode(Program.Client,unicode) ?? DiscordEmoji.FromName(Program.Client, unicode);
        }
        
        public EmojiAttribute(ulong id)
        {
            this.Emoji = DiscordEmoji.FromGuildEmote(Program.Client, id);
        }
    }
}