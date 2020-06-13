using DSharpPlus;
using DSharpPlus.EventArgs;
using sisbase.Interactivity;
using sisbase.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace sisbase.Systems {
	public class InteractivityManager : IClientSystem {
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Status { get; set; }

		public void Activate() {
			Name = "InteractivityManager";
			Description = "Dispatches all Sisbase.Interactivity events.";
			Status = true;
		}
		public async Task ApplyToClient(DiscordClient client) {
			client.MessageUpdated += EditHandler;
			client.MessageReactionAdded += ReactionAddHandler;
			client.MessageReactionRemoved += ReactionRemovedHandler;
			client.MessageDeleted += DeleteHandler;
		}

		private async Task ReactionRemovedHandler(MessageReactionRemoveEventArgs e) {
			foreach (var intr in IMC.InteractionRegistry) {
				await intr.BotMessages.Offer(e);
				await intr.UserMessages.Offer(e);
			}
		}

		private async Task DeleteHandler(MessageDeleteEventArgs e) {
			foreach (var intr in IMC.InteractionRegistry) {
				await intr.BotMessages.Offer(e);
				await intr.UserMessages.Offer(e);
			}
		}

		private async Task ReactionAddHandler(MessageReactionAddEventArgs e) {
			foreach (var intr in IMC.InteractionRegistry) {
				await intr.BotMessages.Offer(e);
				await intr.UserMessages.Offer(e);
			}
		}
		private async Task EditHandler(MessageUpdateEventArgs e) {
			foreach (var intr in IMC.InteractionRegistry) {
				await intr.BotMessages.Offer(e);
				await intr.UserMessages.Offer(e);
			}
		}

		public void Deactivate() {
			Name = null;
			Description = null;
			Status = false;
		}
		public void Execute() {

		}
	}
}
