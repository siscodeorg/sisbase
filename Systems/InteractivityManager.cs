using DSharpPlus;
using DSharpPlus.EventArgs;
using sisbase.Interactivity;
using sisbase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Systems
{
	public class InteractivityManager : IClientSystem
	{
		public string Name { get;set; }
		public string Description { get; set; }
		public bool Status { get;set;}

		public void Activate()
		{
			Name = "InteractivityManager";
			Description = "Dispatches all Sisbase.Interactivity events.";
			Status = true;
		}
		public void ApplyToClient(DiscordClient client)
		{
			client.MessageUpdated += EditHandler;
			client.MessageReactionAdded += ReactionAddHandler;
			client.MessageReactionRemoved += ReactionRemovedHandler;
			client.MessageDeleted += DeleteHandler;
		}

		private async Task ReactionRemovedHandler(MessageReactionRemoveEventArgs e)
		{
			var interaction = IMC.InteractionRegistry.Find(x => x.UserMessages.Contains(e.Message));
			interaction ??= IMC.InteractionRegistry.Find(x => x.BotMessages.Contains(e.Message));
			if (interaction == null) return;
			await interaction.Dispatch(e);
		}

		private async Task DeleteHandler(MessageDeleteEventArgs e)
		{
			var interaction = IMC.InteractionRegistry.Find(x => x.UserMessages.Contains(e.Message));
			interaction ??= IMC.InteractionRegistry.Find(x => x.BotMessages.Contains(e.Message));
			if (interaction == null) return;
			await interaction.Dispatch(e);
		}

		private async Task ReactionAddHandler(MessageReactionAddEventArgs e)
		{
			var interaction = IMC.InteractionRegistry.Find(x => x.UserMessages.Contains(e.Message));
			interaction ??= IMC.InteractionRegistry.Find(x => x.BotMessages.Contains(e.Message));
			if (interaction == null) return;
			await interaction.Dispatch(e);
		}
		private async Task EditHandler(MessageUpdateEventArgs e)
		{
			var interaction = IMC.InteractionRegistry.Find(x => x.UserMessages.Contains(e.Message));
			interaction ??= IMC.InteractionRegistry.Find(x => x.BotMessages.Contains(e.Message));
			if (interaction == null) return;
			await interaction.Dispatch(e);
		}

		public void Deactivate()
		{
			Name = null;
			Description = null;
			Status = false;
		}
		public void Execute()
		{

		}
	}
}
