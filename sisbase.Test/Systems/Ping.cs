using DSharpPlus;
using DSharpPlus.EventArgs;
using sisbase.Utils;
using System.Threading.Tasks;

namespace sisbase.Test.Systems
{
	public class Ping 
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Status { get; set; }

		public void Activate()
		{
			Name = "Ping";
			Description = "Dummy System for teaching how systems work";
			Status = true;
		}

		public async Task ApplyToClient(DiscordClient client)
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

		public void Deactivate()
		{
			Name = null;
			Description = null;
			Status = false;
			SisbaseBot.Instance.Client.MessageCreated -= MessageCreated;
		}

		public void Execute() => Logger.Log("", "This was called inside of an Execute Block");
	}
}