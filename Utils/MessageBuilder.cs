using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace sisbase.Utils
{
	public class MessageBuilder
	{
		public DiscordEmbed Embed { get; internal set; }
		public string Content { get; internal set; }
		public ulong MessageId { get; internal set; } = 0;
		public MessageBuilder(string Content)
			=> WithContent(Content);
		public MessageBuilder(DiscordEmbed embed)
			=> WithEmbed(embed);
		public MessageBuilder()
			=> ClearEmbed().ClearContent();
		public MessageBuilder(DiscordMessage msg)
		{
			Content = msg.Content;
			MessageId = msg.Id;
			if (msg.Embeds.Count > 0)
				Embed = msg.Embeds[0];
		}
		public MessageBuilder WithEmbed(DiscordEmbed embed)
		{
			Embed = embed; return this;
		}

		public MessageBuilder ClearEmbed()
		{
			Embed = null; return this;
		}
		public MessageBuilder ClearContent()
		{
			Content = string.Empty; return this;
		}

		public MessageBuilder WithContent(string content)
		{
			Content = content; return this;
		}

		public async Task<DiscordMessage> Build(DiscordChannel channel, bool tts = false)
		{
			if (MessageId == 0)
			{
				var _msg = await channel.SendMessageAsync(Content, tts, Embed);
				MessageId = _msg.Id;
				return _msg;
			}
			var msg = await channel.GetMessageAsync(MessageId);
			return await msg.ModifyAsync(Content, Embed);
		}
	}
}
