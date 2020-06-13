using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using sisbase.Utils;

namespace sisbase.Configuration {
    public class MainConfig : IConfiguration {
        public string Path { get; set; }
        public MainConfigData Data { get; set; }
        public void Update() {
            File.WriteAllText(Path,JsonConvert.SerializeObject(this,Formatting.Indented));
        }

        public void Create(DirectoryInfo di) {
            Path = $"{di.FullName}/Config.json";
            if (File.Exists(Path)) {
                Data = JsonConvert.DeserializeObject<MainConfig>(File.ReadAllText(Path)).Data;
            }
            else {
                Data = General.TUI_cfg();
                Update();
            }
        }
    }

    public class MainConfigData {
        [JsonProperty] internal string Token { get; set; }
        [JsonProperty] public ulong MasterId { get; set; }
        [JsonProperty] public List<ulong> PuppetId { get; set; } = new List<ulong>();
        [JsonProperty] public List<string> Prefixes { get; set; } = new List<string>();
        [JsonProperty] internal Dictionary<string,object> CustomSettings { get; set; } = new Dictionary<string, object>();
        
    }
}
