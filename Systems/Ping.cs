using DSharpPlus;
using DSharpPlus.EventArgs;
using sisbase.Utils;

namespace sisbase.Systems
{
    // Example on how to implement the custom systems
    public class Ping : IApplyToClient, IApplicableSystem
    {
        public void ApplyToClient(DiscordClient client) => client.MessageCreated += async delegate (MessageCreateEventArgs args)
        {
            if (args.Message.Content == "bot gives ping")
            {
                await args.Message.RespondAsync($"Ping : **{client.Ping}ms**");
            }
        };
    }
}