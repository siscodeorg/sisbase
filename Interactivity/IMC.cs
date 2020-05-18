﻿using DSharpPlus.Entities;
using sisbase.Utils;
using System;
using System.Collections.Generic;

namespace sisbase.Interactivity {
	/// <summary>
	/// Interaction Manager Controller
	/// </summary>
	internal static class IMC {
		static IMC() => InteractionRegistry = new List<Interaction>();
		internal static List<Interaction> InteractionRegistry { get; } = new List<Interaction>();
		internal static void AddInteraction(Interaction interaction)
			=> InteractionRegistry.Add(interaction);
		internal static void RemoveIntraction(Interaction interaction)
			=> InteractionRegistry.Remove(interaction);
		internal static Interaction GetInteraction(DiscordMessage origin)
			=> InteractionRegistry.Find(x => x.Origin == origin);
		internal static void HandleExceptions(string eventName, Exception ex) {
			if (ex is OperationCanceledException) return;
			if (ex is AggregateException age)
				age.Handle(x => { HandleExceptions(eventName, x); return false; });
			else
				Logger.Warn("InteractionAPI", $"An {ex.GetType()} happened in {eventName}.");
		}
	}
}
