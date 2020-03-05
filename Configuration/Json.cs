using Newtonsoft.Json;
using System.Collections.Generic;

namespace sisbase
{
	/// <summary>
	/// The configuration whichs is saved to the config.json file
	/// </summary>
	public class Json
	{
		[JsonProperty] public string Token { get; set; }
		[JsonProperty] public ulong MasterId { get; set; }
		[JsonProperty] public List<ulong?> PuppetId { get; set; }
		[JsonProperty] public List<string> Prefixes { get; set; }
	}
}