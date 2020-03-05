using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using sisbase.Configuration;
using sisbase.Utils;
using System;
using System.Threading.Tasks;

namespace sisbase
{
	public class SisbaseBot : IDisposable
	{
		public static SisbaseBot Instance { get; private set; }
		public Sisbase SisbaseConfiguration { get; private set; }
		public DiscordClient Client { get; private set; }
		public CommandsNextExtension CommandsNext { get; private set; }
		public InteractivityExtension Interactivity { get; private set; }
		public SMC Systems { get; private set; }

		public SisbaseBot(Sisbase sisbaseConfiguration)
		{

			if(Instance != null)
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
					StringPrefixes = SisbaseConfiguration.Config.Prefixes,
					EnableDefaultHelp = false
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
			
			CommandsNext.RegisterCommands(typeof(SisbaseBot).Assembly);
			Systems = new SMC();
			Systems.RegisterSystems(typeof(SisbaseBot).Assembly);

		}

		public Task StartAsync()
			=> Client.ConnectAsync();
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
