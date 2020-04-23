using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace sisbase.Utils
{
	public class MessageBuilder
	{
		public DiscordEmbed Embed { get; internal set; }
		public string Content { get; internal set; }

		public MessageBuilder WithEmbed(DiscordEmbed embed)
		{
			Embed = embed; return this;
		}

		public MessageBuilder ClearEmbed()
		{
			Embed = null; return this;
		}

		public MessageBuilder WithContent(string content)
		{
			Content = content; return this;
		}

		public async Task<DiscordMessage> Build(DiscordChannel channel, bool tts = false) => await channel.SendMessageAsync(Content, tts, Embed);
	}
}