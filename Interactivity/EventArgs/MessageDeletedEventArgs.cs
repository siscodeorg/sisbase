using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace sisbase.Interactivity.EventArgs
{
	public class MessageDeletedEventArgs : DiscordEventArgs
	{
		public InteractionMessage Message { get; internal set; }
		public DiscordChannel Channel
			=> Message.Channel;
		internal MessageDeletedEventArgs(DiscordClient client) : base(client) { }

		internal MessageDeletedEventArgs(MessageDeleteEventArgs dspargs, InteractionMessage owner) : base(dspargs.Client)
		{
			Message = owner;
		}
	}
}
