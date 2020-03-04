using DSharpPlus;
using DSharpPlus.EventArgs;
using sisbase.Utils;
using System;

namespace sisbase.Systems
{
    // Example on how to implement the custom systems
    public class Ping : IApplyToClient
    {
        public string Name {get;set;}
        public string Description {get;set;}
        public bool Status {get;set;}

        public void Activate() 
        {
            Name = "Ping";
            Description = "Dummy System for teaching how systems work";
            Status = true;
        }
        public void ApplyToClient(DiscordClient client) => client.MessageCreated += async delegate (MessageCreateEventArgs args)
        {
            if (args.Message.Content == "bot gives ping")
            {
                await args.Message.RespondAsync($"Ping : **{client.Ping}ms**");
            }
        };
        public void Deactivate() 
        {
            Name = null;
            Description = null;
            Status = false;
            GC.SuppressFinalize(this);
        }
        public void Execute() => Console.WriteLine("This was called inside of an Execute Block");
    }
}