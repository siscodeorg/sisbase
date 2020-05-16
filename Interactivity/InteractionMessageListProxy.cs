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
		public InteractionMessage Get(InteractionMessageProxyMode mode)
		{
            if (Mode == InteractionMessageListProxyMode.BOT)
            {
                if (mode == InteractionMessageProxyMode.FIRST)
                    return Parent.BotMessages.FirstOrDefault();
                else
                    return Parent.BotMessages.LastOrDefault();
            }
            else
            {
                if (mode == InteractionMessageProxyMode.FIRST)
                    return Parent.UserMessages.FirstOrDefault();
                else
                    return Parent.UserMessages.LastOrDefault();
            }

        }
	}
}
