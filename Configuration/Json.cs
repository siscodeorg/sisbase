using Newtonsoft.Json;
using System.Collections.Generic;

namespace sisbase {
	/// <summary>
	/// The configuration whichs is saved to the config.json file
	/// </summary>
	public class Json {
		[JsonProperty] internal string Token { get; set; }

		/// <summary>
		/// The Id of the master server
		/// </summary>
		[JsonProperty] public ulong MasterId { get; set; }

		/// <summary>
		/// The Ids of the puppet servers
		/// </summary>
		[JsonProperty] public List<ulong?> PuppetId { get; set; }

		/// <summary>
		/// The prefixes that are going to be usedby the bot.
		/// </summary>
		[JsonProperty] public List<string> Prefixes { get; set; }

		[JsonProperty] internal Dictionary<string, object> CustomSettings { get; set; }
	}
}