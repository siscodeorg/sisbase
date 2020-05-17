using DSharpPlus;
using DSharpPlus.Interactivity;
using sisbase.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Test.Systems
{
	public class testbedsys : IClientSystem
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Status { get; set; }

		public void Activate()
		{
			Name = "TestBed System";
			Description = "Used to test random d#+ calls";
			Status = true;
		}

		public async Task ApplyToClient(DiscordClient client)
		{
			client.MessageCreated += MessageCreated;
		}

		private async Task MessageCreated(DSharpPlus.EventArgs.MessageCreateEventArgs e)
		{
			if (e.Message.Content == "tb!gnmdm")
			{
				var member = await e.Guild.GetMemberAsync(e.Message.Author.Id);
				await Task.Factory.StartNew(async () =>
				{
					var dmc = await member.CreateDmChannelAsync();
					var msg = await dmc.SendMessageAsync("ndmm : AWAITING");
					var response = await dmc.GetNextMessageAsync(member);
					await msg.ModifyAsync($"ndmm : \n{response.Result.Content}");
				});
			}
		}

		public void Deactivate()
		{
		}

		public void Execute()
		{
		}
	}
}