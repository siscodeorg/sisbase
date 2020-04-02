using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using sisbase.Configuration;
using sisbase.Utils;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace sisbase
{
	/// <summary>
	/// The class all sisbase bots derive from
	/// </summary>
	public class SisbaseBot : IDisposable
	{
		/// <summary>
		/// The currently running Instance
		/// </summary>
		public static SisbaseBot Instance { get; private set; }

		/// <summary>
		/// The configuration for the bot.
		/// </summary>
		public Sisbase SisbaseConfiguration { get; private set; }

		/// <summary>
		/// The DiscordClient
		/// </summary>
		public DiscordClient Client { get; private set; }

		public CommandsNextExtension CommandsNext { get; private set; }

		public InteractivityExtension Interactivity { get; private set; }
		public SMC Systems { get; private set; }

		public SisbaseBot(Sisbase sisbaseConfiguration)
		{
			if (Instance != null)
				throw new InvalidOperationException("Instance is already running");
			Instance = this;
			SisbaseConfiguration = sisbaseConfiguration;
			Client = new DiscordClient(
				new DiscordConfiguration
				{
					AutoReconnect = true,
					Token = SisbaseConfiguration.Config.Token,
					UseInternalLogHandler = false
				}
			);
			CommandsNext = Client.UseCommandsNext(
				new CommandsNextConfiguration
				{
					EnableDefaultHelp = false,
					PrefixResolver = RTPR
				}
			);
			Interactivity = Client.UseInteractivity(
				new InteractivityConfiguration
				{
					Timeout = TimeSpan.FromMinutes(15),
					PaginationBehaviour = PaginationBehaviour.WrapAround,
					PaginationDeletion = PaginationDeletion.DeleteEmojis,
					PollBehaviour = PollBehaviour.DeleteEmojis
				}
			);

			Systems = new SMC();
			Systems.RegisterSystems(typeof(SisbaseBot).Assembly);
			CommandsNext.RegisterCommands(typeof(SisbaseBot).Assembly);
		}

		/// <summary>
		/// Real-Time Prefix Resolver
		/// </summary>
#pragma warning disable CS1998

		private async Task<int> RTPR(DiscordMessage msg)
		{
			switch (msg.GetMentionPrefixLength(Instance.Client.CurrentUser))
			{
				case -1:
					int x;
					foreach (string prefix in Instance.SisbaseConfiguration.Config.Prefixes)
					{
						x = msg.GetStringPrefixLength(prefix);
						if (x != -1)
							return x;
					}

					break;

				default:
					return msg.GetMentionPrefixLength(Instance.Client.CurrentUser);
			}

			return -1;
		}

		/// <summary>
		/// Registers all systems and commands from a given assembly. The System and Command classes need to be public for registration
		/// </summary>
		/// <param name="asm">The assembly</param>
		public void RegisterBot(Assembly asm)
		{ Systems.RegisterSystems(asm); CommandsNext.RegisterCommands(asm); }

#pragma warning restore CS1998

		/// <summary>
		/// Starts the bot instance
		/// </summary>
		/// <returns></returns>
		public Task StartAsync()
			=> Client.ConnectAsync();

		/// <summary>
		/// Disconnects the bot
		/// </summary>
		/// <returns></returns>
		public Task DisconnectAsync()
			=> Client.DisconnectAsync();

		~SisbaseBot() =>
			Dispose(false);

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Client.Dispose();
			}
		}
	}
}