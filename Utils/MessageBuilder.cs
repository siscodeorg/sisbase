using DSharpPlus.Entities;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace sisbase.Utils
{
	public class MessageBuilder
	{
		public DiscordEmbed Embed { get; internal set; }
		public string Content { get; internal set; }
		public ulong MessageId { get; internal set; } = 0;
		public bool TTS { get; set; }
		public List<IMention> Mentions { get;internal set; }
		public MessageBuilder(string Content)
			=> WithContent(Content);
		public MessageBuilder(DiscordEmbed embed)
			=> WithEmbed(embed);
		public MessageBuilder()
			=> ClearEmbed().ClearContent();
		internal Stream Data = Stream.Null;
		internal string FilePath;
		internal Dictionary<string, Stream> ManyData = new Dictionary<string, Stream>();
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
		public MessageBuilder WithMentions(List<IMention> mentions)
		{
			Mentions = mentions; return this;
		}
		public MessageBuilder ClearMentions()
		{
			Mentions.Clear(); return this;
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
		public MessageBuilder SetTTS(bool tts)
		{
			TTS = tts; return this;
		}
		public MessageBuilder Bind(Stream data)
		{
			Data = data; return this;
		}
		public MessageBuilder Bind(string filePath)
		{
			FilePath = filePath; return this;
		}
		public MessageBuilder BindMany(Dictionary<string,Stream> multipleFiles)
		{
			ManyData = multipleFiles; return this;
		}

		public async Task<DiscordMessage> Build(DiscordChannel channel)
		{
			if (MessageId == 0)
			{
				DiscordMessage _msg;
				if (ManyData.Count > 0)
					_msg = await channel.SendMultipleFilesAsync(ManyData, Content, TTS, Embed, Mentions);
				else if (!string.IsNullOrEmpty(FilePath))
					_msg = await channel.SendFileAsync(FilePath, Content, TTS, Embed, Mentions);
				else if (Data != Stream.Null)
					_msg = await channel.SendFileAsync(Data as FileStream, Content, TTS, Embed, Mentions);
				else
					_msg = await channel.SendMessageAsync(Content, TTS, Embed, Mentions);

				MessageId = _msg.Id;
				return _msg;
			}
			var msg = await channel.GetMessageAsync(MessageId);
			return await msg.ModifyAsync(Content, Embed);
		}
	}
}
