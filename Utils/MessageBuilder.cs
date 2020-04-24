using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace sisbase.Utils
{
	public class MessageBuilder
	{
		public DiscordEmbed Embed { get; internal set; }
		public string Content { get; internal set; }
		public ulong MessageId { get; internal set; } = 0;

		public MessageBuilder()
		{
		}

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

		public MessageBuilder WithContent(string content)
		{
			Content = content; return this;
		}

		public async Task<DiscordMessage> Build(DiscordChannel channel, bool tts = false)
		{
			if (MessageId == 0)
			{
				var msg = await channel.SendMessageAsync(Content, tts, Embed);
				MessageId = msg.Id;
				return msg;
			}

			return await (await channel.GetMessageAsync(MessageId)).ModifyAsync(Content, Embed);
		}
	}
}