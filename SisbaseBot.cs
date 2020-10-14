using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using sisbase.Configuration;
using sisbase.Systems;
using sisbase.Utils;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace sisbase
{
#pragma warning disable CS1591

	/// <summary>
	/// The class all sisbase bots derive from
	/// </summary>
	public class SisbaseBot : IDisposable
	{
		/// <summary>
		/// The currently running Instance
		/// </summary>
		public static SisbaseBot Instance { get; private set; }

		internal static CancellationTokenSource _cts;

		/// <summary>
		/// The configuration for the bot.
		/// </summary>
		public MainConfig SisbaseConfiguration { get; } = new MainConfig();

		public SystemConfig SystemCfg { get; } = new SystemConfig();

		/// <summary>
		/// The DiscordClient
		/// </summary>
		public DiscordClient Client { get; private set; }

		/// <summary>
		/// The CommandsNextExtension for the <see cref="Client"/>
		/// </summary>
		public CommandsNextExtension CommandsNext { get; private set; }

		/// <summary>
		/// The InteractivityExtension for the <see cref="Client"/>
		/// </summary>
		public InteractivityExtension Interactivity { get; private set; }

		/// <summary>
		/// The System Managment Controller <br></br> Responsible for registry and unregistry of all Systems.
		/// </summary>
		public SMC Systems { get; private set; }

		/// <summary>
		/// Constructs a new <see cref="SisbaseBot"/> from a given configuration
		/// </summary>
		/// <param name="configDirectory">The directory used to store the configuration.</param>
		public SisbaseBot(string configDirectory)
		{
			if (Instance != null)
				throw new InvalidOperationException("Instance is already running");
			Instance = this;
			SisbaseConfiguration.Create(Directory.CreateDirectory(Directory.GetCurrentDirectory()));
			CreateNewBot();
			SystemCfg.Create(Directory.CreateDirectory(configDirectory));
		}

		public SisbaseBot()
		{
			if (Instance != null)
				throw new InvalidOperationException("Instance is already running");
			Instance = this;
			SisbaseConfiguration.Create(Directory.CreateDirectory(Directory.GetCurrentDirectory()));
			CreateNewBot();
			SystemCfg.Create(Directory.CreateDirectory(Directory.GetCurrentDirectory()));
		}

		internal void CreateNewBot()
		{
			Client = new DiscordClient(
				new DiscordConfiguration
				{
					AutoReconnect = true,
					Token = SisbaseConfiguration.Data.Token,
					MinimumLogLevel = LogLevel.Information
				}
			);
			CommandsNext = Client.UseCommandsNext(
				new CommandsNextConfiguration
				{
					EnableDefaultHelp = false,
					PrefixResolver = RTPR,
					Services = new ServiceCollection().AddSingleton(this).BuildServiceProvider()
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
			_cts = new CancellationTokenSource();
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
					foreach (string prefix in Instance.SisbaseConfiguration.Data.Prefixes)
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
		/// Registers all systems and commands from a given assembly. The System and Command classes
		/// need to be public for registration
		/// </summary>
		/// <param name="asm">The assembly</param>
		public void RegisterBot(Assembly asm)
		{ Systems.RegisterSystems(asm); CommandsNext.RegisterCommands(asm); }

#pragma warning restore CS1998
		/// <summary>
		/// Starts the bot instance
		/// </summary>
		/// <returns></returns>
		public async Task Start()
		{
			Console.CancelKeyPress += (sender, e) =>
			{
				if (!_cts.IsCancellationRequested)
					Stop();
				e.Cancel = true;
			};
			await Connect();
			Client.GuildDownloadCompleted += async (c,a) => { Logger.Log("DSharpPlus","The bot is ready for usage. [GuildDownloadCompleted]");};
			await _cts.Token.WhenCanceled();
		}

		public void Stop() => _cts.Cancel();

		internal Task Connect()
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