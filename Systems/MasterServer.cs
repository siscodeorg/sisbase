using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using sisbase.Attributes;
using sisbase.Utils;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace sisbase.Systems
{
	/// <summary>
	/// The guild the bot is supossed to run
	/// </summary>
#pragma warning disable CS1591
	[Vital]
	public class MasterServer : IClientSystem
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Status { get; set; }

		public void Activate()
		{
			Name = "MasterServer";
			Description = "MasterServer System";
			Status = true;
		}

		public async Task ApplyToClient(DiscordClient client) => client.GuildDownloadCompleted += async delegate (GuildDownloadCompletedEventArgs args)
		{
			if (SisbaseBot.Instance.SisbaseConfiguration.Data.MasterId == 0)
			{
				if (client.Guilds.Count > 1)
				{
					string names = client.Guilds.Aggregate("", (current, guild) => current + $"{guild.Value.Name}, ");

					foreach (var guild in client.Guilds)
					{
						var ch = guild.Value.GetDefaultChannel();
						var builder = new DiscordEmbedBuilder();

						builder
							.AddField("List of Servers", names)
							.WithDescription($"Bot is loaded on multiple servers, please use {client.CurrentUser.Mention}setMaster on the MASTER guild.")
							.WithAuthor("Error on determining MASTER server")
							.WithColor(DiscordColor.Red);
						await ch.SendMessageAsync(embed: builder);
					}
				}
				else
				{
					var ch = client.Guilds.Values.ToList()[0].GetDefaultChannel();
					var builder = new DiscordEmbedBuilder();
					builder
						.WithAuthor("Guild set as MASTER guild")
						.WithColor(DiscordColor.PhthaloGreen);
					SisbaseBot.Instance.SisbaseConfiguration.Data.MasterId = client.Guilds.Values.ToList()[0].Id;
					SisbaseBot.Instance.SisbaseConfiguration.Update();
					await ch.SendMessageAsync(embed: builder);
				}
			}
		};

		public void Deactivate()
		{
			Name = null;
			Description = null;
			Status = false;
			GC.SuppressFinalize(this);
		}

		public void Execute()
		{
		}
	}
}