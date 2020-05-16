using sisbase.Interactivity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sisbase.Interactivity
{
	public class InteractionMessageListProxy
	{
		public InteractionMessageListProxyMode Mode { get; internal set; }
		internal Interaction Parent;
		public List<InteractionMessage> Get() 
			=> Mode == InteractionMessageListProxyMode.BOT ? Parent.BotMessages : Parent.UserMessages;
	}
}
