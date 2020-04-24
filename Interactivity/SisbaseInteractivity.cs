using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using sisbase.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Interactivity
{
	public static class SisbaseInteractivity
	{
		/// <summary>
		/// Interacts with a specific <see cref="DiscordMember"/> and waits for said member response
		/// on a specified <see cref="DiscordChannel"/>.
		/// </summary>
		/// <param name="member">Member that the bot will interact with</param>
		/// <param name="embed">The Embed that will be sent as a message</param>
		/// <param name="channel">The channel of the interaction</param>
		/// <returns>The <see cref="DiscordMessage"/> sent by the member.</returns>
		public static async Task<DiscordMessage> InteractAsync(this DiscordMember member, DiscordEmbed embed, DiscordChannel channel)
		{
			await new MessageBuilder().WithEmbed(embed).Build(channel);
			var data = await channel.GetNextMessageAsync(member);
			return data.Result;
		}

		/// <summary>
		/// Interacts with a specific <see cref="DiscordMember"/> and waits for said member response
		/// on a specified <see cref="DiscordChannel"/>.
		/// </summary>
		/// <param name="member">Member that the bot will interact with</param>
		/// <param name="content">The content that will be sent as a message</param>
		/// <param name="channel">The channel of the interaction</param>
		/// <returns>The <see cref="DiscordMessage"/> sent by the member.</returns>
		public static async Task<DiscordMessage> InteractAsync(this DiscordMember member, string content, DiscordChannel channel)
		{
			await new MessageBuilder().WithContent(content).Build(channel);
			var data = await channel.GetNextMessageAsync(member);
			return data.Result;
		}

		/// <summary>
		/// Interacts with a specific <see cref="DiscordMember"/> and waits for said member response
		/// on a specified <see cref="DiscordChannel"/>.
		/// </summary>
		/// <param name="member">Member that the bot will interact with</param>
		/// <param name="message">A lambda that defines an <see cref="MessageBuilder"/></param>
		/// <param name="channel">The channel of the interaction</param>
		/// <returns>The <see cref="DiscordMessage"/> sent by the member.</returns>
		public static async Task<DiscordMessage> InteractAsync(this DiscordMember member, Action<MessageBuilder> message, DiscordChannel channel)
		{
			var builder = new MessageBuilder();
			message(builder);
			await builder.Build(channel);
			var data = await channel.GetNextMessageAsync(member);
			return data.Result;
		}
	}
}