using DSharpPlus;
using sisbase.Interactivity;
using sisbase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static sisbase.Interactivity.EventArgs;

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
			client.MessageDeleted += DeleteHandler;
		}

		private async Task DeleteHandler(DSharpPlus.EventArgs.MessageDeleteEventArgs e)
		{
			if (!IMC.InteractionRegistry.Any(x => x.UserMessages.Contains(e.Message))) return;
			var interaction = IMC.InteractionRegistry.Find(x => x.UserMessages.Contains(e.Message));
			interaction.InvokeEvent(new MessageDeleteArgs(e.Message));
		}

		private async Task ReactionAddHandler(DSharpPlus.EventArgs.MessageReactionAddEventArgs e)
		{
			if (!IMC.InteractionRegistry.Any(x => x.UserMessages.Contains(e.Message))) return;
			var interaction = IMC.InteractionRegistry.Find(x => x.UserMessages.Contains(e.Message));
			interaction.InvokeEvent(new MessageReactArgs(e.Message, e.Emoji,e.User));
		}
		private async Task EditHandler(DSharpPlus.EventArgs.MessageUpdateEventArgs e)
		{
			if (!IMC.InteractionRegistry.Any(x => x.UserMessages.Contains(e.Message))) return;
			var interaction = IMC.InteractionRegistry.Find(x => x.UserMessages.Contains(e.Message));
			interaction.InvokeEvent(new MessageEditArgs(e.MessageBefore,e.Message)) ;
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
