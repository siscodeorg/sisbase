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
                Data.CommandSettings.Groups["config"] = new GroupSetting {
                    Enabled = true
                };
                Data.CommandSettings.Groups["system"] = new GroupSetting {
                    Enabled = true
                };
                Update();
            }
        }

        public T GetConfig<T>(string Path) 
            => (T) Data.CustomSettings[Path];

        public void SetConfig<T>(string Path, T value)
            => Data.CustomSettings[Path] = value;
    }

    public class MainConfigData {
        [JsonProperty] internal string Token { get; set; }
        [JsonProperty] public ulong MasterId { get; set; }
        [JsonProperty] public List<ulong> PuppetId { get; set; } = new List<ulong>();
        [JsonProperty] public List<string> Prefixes { get; set; } = new List<string>();
        [JsonProperty] public bool EnableDspLogger { get; set; }
        [JsonProperty] public GroupCommandData CommandSettings { get; set; } = new GroupCommandData();
        [JsonProperty] internal Dictionary<string,object> CustomSettings { get; set; } = new Dictionary<string, object>();
        
    }

    public class GroupCommandData {
        public bool EnableSisbaseHelp { get; set; } = true;
        public Dictionary<string, GroupSetting> Groups { get; set; } = new Dictionary<string, GroupSetting>();
    }
    public class GroupSetting {
        //public string Name { get; set; } TBA
        public bool Enabled { get; set; } = true;
    }
}
