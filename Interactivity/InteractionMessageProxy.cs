using DSharpPlus;
using DSharpPlus.Entities;
using sisbase.Interactivity.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace sisbase.Interactivity
{
    public class InteractionMessageProxy
    {
        public InteractionMessageProxyMode Mode { get; internal set; }
        internal InteractionMessageListProxy Parent;
        public InteractionMessage Get()
            => Mode == InteractionMessageProxyMode.FIRST ? Parent.Get().FirstOrDefault() : Parent.Get().LastOrDefault();

        #region delegate
        public DiscordChannel Channel => Get().Channel;
        public ulong ChannelId => Get().ChannelId;
        public DiscordUser Author => Get().Author;
        public string Content => Get().Content;
        public DateTimeOffset Timestamp => Get().Timestamp;
        public DateTimeOffset? EditedTimestamp => Get().EditedTimestamp;
        public bool IsEdited => Get().IsEdited;
        public bool IsTTS => Get().IsTTS;
        public bool MentionEveryone => Get().MentionEveryone;
        public IReadOnlyList<DiscordUser> MentionedUsers => Get().MentionedUsers;
        public IReadOnlyList<DiscordRole> MentionedRoles => Get().MentionedRoles;
        public IReadOnlyList<DiscordChannel> MentionedChannels => Get().MentionedChannels;
        public IReadOnlyList<DiscordAttachment> Attachments => Get().Attachments;
        public IReadOnlyList<DiscordEmbed> Embeds => Get().Embeds;
        public IReadOnlyList<DiscordReaction> Reactions => Get().Reactions;
        public bool Pinned => Get().Pinned;
        public ulong? WebhookId => Get().WebhookId;
        public MessageType? MessageType => Get().MessageType;
        public DiscordMessageActivity Activity => Get().Activity;
        public DiscordMessageApplication Application => Get().Application;
        public DiscordMessageReference Reference => Get().Reference;
        public MessageFlags? Flags => Get().Flags;
        public bool WebhookMessage => Get().WebhookMessage;
        public Uri JumpLink => Get().JumpLink;

        public Task<InteractionMessage> ModifyAsync(Optional<string> content = default,
            Optional<DiscordEmbed> embed = default) => Get().ModifyAsync(content, embed);

        public Task DeleteAsync(string reason = null) => Get().DeleteAsync(reason);
        public Task ModifyEmbedSuppressionAsync(bool hideEmbeds) => Get().ModifyEmbedSuppressionAsync(hideEmbeds);
        public Task PinAsync() => Get().PinAsync();
        public Task UnpinAsync() => Get().UnpinAsync();

        public Task<InteractionMessage> RespondAsync(string content = null, bool tts = false, DiscordEmbed embed = null,
            IEnumerable<IMention> mentions = null) => Get().RespondAsync(content, tts, embed, mentions);

        public Task<InteractionMessage> RespondWithFileAsync(string fileName, Stream fileData, string content = null,
            bool tts = false, DiscordEmbed embed = null, IEnumerable<IMention> mentions = null) =>
            Get().RespondWithFileAsync(fileName, fileData, content, tts, embed, mentions);

        public Task<InteractionMessage> RespondWithFileAsync(FileStream fileData, string content = null,
            bool tts = false, DiscordEmbed embed = null, IEnumerable<IMention> mentions = null) =>
            Get().RespondWithFileAsync(fileData, content, tts, embed, mentions);

        public Task<InteractionMessage> RespondWithFileAsync(string filePath, string content = null,
            bool tts = false, DiscordEmbed embed = null, IEnumerable<IMention> mentions = null) =>
            Get().RespondWithFileAsync(filePath, content, tts, embed, mentions);

        public Task<InteractionMessage> RespondWithFilesAsync(Dictionary<string, Stream> files, string content = null,
            bool tts = false, DiscordEmbed embed = null, IEnumerable<IMention> mentions = null) =>
            Get().RespondWithFilesAsync(files, content, tts, embed, mentions);

        public Task CreateReactionAsync(DiscordEmoji emoji) => Get().CreateReactionAsync(emoji);
        public Task DeleteOwnReactionAsync(DiscordEmoji emoji) => Get().DeleteOwnReactionAsync(emoji);

        public Task DeleteReactionAsync(DiscordEmoji emoji, DiscordUser user, string reason = null) =>
            Get().DeleteReactionAsync(emoji, user, reason);

        public Task<IReadOnlyList<DiscordUser>> GetReactionsAsync(DiscordEmoji emoji, int limit = 25,
            ulong? after = null) => Get().GetReactionsAsync(emoji, limit, after);

        public Task DeleteAllReactionsAsync(string reason = null) => Get().DeleteAllReactionsAsync(reason);
        public Task DeleteReactionsEmojiAsync(DiscordEmoji emoji) => Get().DeleteReactionsEmojiAsync(emoji);
        public override string ToString() => Get().ToString();
        #endregion

    }
}