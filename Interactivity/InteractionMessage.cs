using DSharpPlus;
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
		public DiscordMessage _Message;
		internal Interaction _Owner;
		#region Properties
		public DiscordMessageActivity Activity => _Message.Activity;
		public DiscordMessageApplication Application => _Message.Application;
		public IReadOnlyList<DiscordAttachment> Attachments => _Message.Attachments;
		public DiscordUser Author => _Message.Author;
		public DiscordChannel Channel => _Message.Channel;
		public ulong ChannelId => _Message.ChannelId;
		public string Content => _Message.Content;
		public DateTimeOffset? EditedTimestamp => _Message.EditedTimestamp;
		public IReadOnlyList<DiscordEmbed> Embeds => _Message.Embeds;
		public MessageFlags? Flags => _Message.Flags;
		public bool IsEdited => _Message.IsEdited;
		public bool IsTTS => _Message.IsTTS;
		public Uri JumpLink => _Message.JumpLink;
		public IReadOnlyList<DiscordChannel> MentionedChannels => _Message.MentionedChannels;
		public IReadOnlyList<DiscordRole> MentionedRoles => _Message.MentionedRoles;
		public IReadOnlyList<DiscordUser> MentionedUsers => _Message.MentionedUsers;
		public bool MentionEveryone => _Message.MentionEveryone;
		public MessageType? MessageType => _Message.MessageType;
		public bool Pinned => _Message.Pinned;
		public IReadOnlyList<DiscordReaction> Reactions => _Message.Reactions;
		public DiscordMessageReference Reference => _Message.Reference;
		public DateTimeOffset? Timestamp => _Message.Timestamp;
		public ulong? WebhookId => _Message.WebhookId;
		public bool WebhookMessage => _Message.WebhookMessage;
		public ulong Id => _Message.Id;
		#endregion
		
		//public async Task Delete()
		//	=> await _Owner.Remove(this);

		public async Task MutateAsync(Action<MessageBuilder> action)
		{
			var builder = new MessageBuilder(_Message);
			action(builder);
			_Message = await builder.Build(_Message.Channel);
		}
		public Task RespondAsync(MessageBuilder message) 
			=>  _Owner.SendMessageAsync(message);
		public async Task ToggleReactionAsync(DiscordEmoji reaction)
		{
			if ((await _Message.GetReactionsAsync(reaction)).Contains(SisbaseBot.Instance.Client.CurrentUser))
				await _Message.DeleteOwnReactionAsync(reaction);
			else
				await _Message.CreateReactionAsync(reaction);
		}
		public Task CreateReactionAsync(DiscordEmoji emoji) 
			=> _Message.CreateReactionAsync(emoji);
		public Task DeleteOwnReactionAsync(DiscordEmoji emoji) 
			=> _Message.DeleteOwnReactionAsync(emoji);
		public Task ModifyEmbedSuppressionAsync(bool hideEmbeds) 
			=> _Message.ModifyEmbedSuppressionAsync(hideEmbeds);
		public Task PinAsync() 
			=> _Message.PinAsync();
		public Task UnpinAsync()
			=> _Message.UnpinAsync();
		public Task DeleteReactionAsync(DiscordEmoji emoji, DiscordUser user, string reason = null) =>
			_Message.DeleteReactionAsync(emoji, user, reason);

		public InteractionMessage(DiscordMessage m, Interaction i)
		{
			_Message = m;
			_Owner = i;
		}
	}
}
