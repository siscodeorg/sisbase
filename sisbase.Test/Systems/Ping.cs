using DSharpPlus;
using DSharpPlus.EventArgs;
using sisbase.Systems;
using sisbase.Utils;
using System.Threading.Tasks;

namespace sisbase.Test.Systems
{
	public class Ping : ClientSystem
	{
		public override async Task Activate()
		{
			Name = "Ping";
			Description = "Dummy System for teaching how systems work";
			Status = true;
		}

		public override async Task ApplyToClient(DiscordClient client)
		{
			client.MessageCreated += MessageCreated;
		}

		private async Task MessageCreated(DiscordClient c, MessageCreateEventArgs e)
		{
			if (e.Message.Content == "bot gives ping")
			{
				await e.Message.RespondAsync($"Ping : **{c.Ping}ms**");
				Logger.Log("",$"{e.Message.Author.Username} requested the ping");
			}
		}

		public override async Task Deactivate()
		{
			Name = null;
			Description = null;
			Status = false;
			SisbaseBot.Instance.Client.MessageCreated -= MessageCreated;
		}
		public override async Task<bool> CheckPreconditions() {
			this.Log("This was called inside of an CheckPreconditions Block");
			return true;
		}
	}
}
