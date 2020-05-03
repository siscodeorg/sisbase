using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace sisbase.Interactivity
{
	/// <summary>
	/// Interaction Manager Controller
	/// </summary>
	internal static class IMC 
	{
		static IMC() => InteractionRegistry = new List<Interaction>();
		internal static List<Interaction> InteractionRegistry { get; } = new List<Interaction>();
		internal static void AddInteraction(Interaction interaction)
			=> InteractionRegistry.Add(interaction);
		internal static void RemoveIntraction(Interaction interaction)
			=> InteractionRegistry.Remove(interaction);
		internal static Interaction GetInteraction(DiscordMessage origin)
			=> InteractionRegistry.Find(x => x.Origin == origin);
	}
}
