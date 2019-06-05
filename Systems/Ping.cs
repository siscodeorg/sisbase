using System;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using LA_RPbot.Discord.Utils;

namespace LA_RPbot.Discord.Systems
{
    // Example on how to implement the custom systems
    public class Ping : IApplyToInteractivity, IApplicableSystem
    {
        public void ApplyToInteractivity(InteractivityExtension interactivity)
        {
            interactivity.Client.MessageCreated += async delegate(MessageCreateEventArgs args)
            {
                if (args.Message.Content == "bot gives ping")
                {
                    await args.Message.RespondAsync($"Ping : **{interactivity.Client.Ping}ms**");
                }
            };
        }
    }
}