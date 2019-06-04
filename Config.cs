using System.Collections.Generic;
using Newtonsoft.Json;

namespace LA_RPbot.Discord
{
    public class Config
    {
        [JsonProperty] public string Token { get; set; }
        [JsonProperty] public ulong MasterId { get; set; }
        [JsonProperty] public List<ulong?> PuppetId { get; set; }
        [JsonProperty] public List<string> Prefixes { get; set; }
    }
}