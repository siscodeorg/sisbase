using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using sisbase.Utils;

namespace sisbase.Configuration {
    public class MainConfig : IConfiguration {
        public string Path { get; set; }
        [JsonProperty] internal int ConfigVersion { get; set; } = 2;
        public MainConfigData Data { get; set; }
        public void Update() {
            File.WriteAllText(Path,JsonConvert.SerializeObject(this,Formatting.Indented));
        }

        public void Create(DirectoryInfo di) {
            Path = $"{di.FullName}/Config.json";
            if (File.Exists(Path)) {
                try {
                    var jtoken = JsonConvert.DeserializeObject<JToken>(File.ReadAllText(Path));
                    if (IsLegacy(jtoken)) {
                        UpdateLegacyFormats(jtoken);
                    }
                    else {
                        Data = jtoken["Data"].ToObject<MainConfigData>();
                    }
                }
                catch (JsonException ex) {
                    Logger.Warn("sisbase", "The provided config file is malformed!");
                    Logger.Warn("sisbase", $"Details :\n    {ex.Message}");
                    if (File.Exists($"{Path}.err"))
                        File.Delete($"{Path}.err");
                    File.Move(Path, $"{Path}.err");
                    Data = new MainConfigData();
                    Update();
                    Logger.Warn("sisbase", $"Bot config was set to default values. Old config available on {Path}.err");
                    Logger.Log("sisbase", $"Bot will now exit. Please edit the config @ {Path}");
                    Environment.Exit(-1);
                }
            }
            else {
                Data = General.TUI_cfg();
                Update();
            }
        }
        internal void UpdateLegacyFormats(JToken config) {
            Logger.Log("sisbase", "Outdated Config.json format. Updating process has begun.");
            File.WriteAllText($"{Path}.backup",JsonConvert.SerializeObject(config,Formatting.Indented));
            if (config["ConfigVersion"] == null) { //First config. (sisbase 1.0)
                Logger.Log("sisbase", "ConfigVersion [Unknown] -> 2");
                dynamic ndat = config.ToObject<MainConfigData>();
                if (ndat == default(MainConfigData)) {
                    Logger.Warn("sisbase", $"Couldn't convert Config.json to Version 2");
                    Data = new MainConfigData();
                }else {
                    Data = ndat;
                }
                Update();
            }
        }
        internal bool IsLegacy(JToken config) =>
            config["ConfigVersion"] == null || config["ConfigVersion"].ToObject<int>() < 2;
    }

    public class MainConfigData {
        [JsonProperty] internal string Token { get; set; } = "";
        [JsonProperty] public ulong MasterId { get; set; } = 0;
        [JsonProperty] public List<ulong> PuppetId { get; set; } = new List<ulong>();
        [JsonProperty] public List<string> Prefixes { get; set; } = new List<string>();
        [JsonProperty] internal Dictionary<string, object> CustomSettings { get; set; } = new Dictionary<string, object>();
    }
}
