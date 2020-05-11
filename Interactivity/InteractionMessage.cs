using DSharpPlus.Entities;
using sisbase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sisbase.Interactivity
{
	public class InteractionMessage 
	{
		internal DiscordMessage _Message;
		internal Interaction _Owner;
		//public async Task Delete()
		//	=> await _Owner.Remove(this);
		public string Content => _Message.Content;
		public DiscordUser Author => _Message.Author;
		public DiscordChannel Channel => _Message.Channel;
		public IReadOnlyList<DiscordAttachment> Attachments => _Message.Attachments;
		public IReadOnlyList<DiscordEmbed> Embeds => _Message.Embeds;
		public ulong Id => _Message.Id;
		public async Task Mutate(Action<MessageBuilder> action)
		{
			var builder = new MessageBuilder(_Message);
			action(builder);
			await builder.Build(_Message.Channel);
		}
		public async Task Respond(MessageBuilder message) 
			=> await _Owner.SendMessageAsync(message);
		public async Task ToggleReaction(DiscordEmoji reaction)
		{
			if ((await _Message.GetReactionsAsync(reaction)).Contains(SisbaseBot.Instance.Client.CurrentUser))
				await _Message.DeleteOwnReactionAsync(reaction);
			else
				await _Message.CreateReactionAsync(reaction);
		}
		public async Task AddReaction(DiscordEmoji reaction) 
			=> await _Message.CreateReactionAsync(reaction);
		public async Task RemoveReaction(DiscordEmoji reaction)
			=> await _Message.DeleteOwnReactionAsync(reaction);
		internal InteractionMessage(DiscordMessage m, Interaction i)
		{
			_Message = m;
			_Owner = i;
		}
	}
}
