using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using sisbase.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace sisbase.Configuration {
    public class MainConfig : IConfiguration {
        public string Path { get; set; }
        [JsonProperty] internal int ConfigVersion { get; set; } = 2;

        public MainConfigData Data { get; private set; }
        public void Update() {
            File.WriteAllText(Path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public void Create(DirectoryInfo di) {
            Path = $"{di.FullName}/Config.json";
            if (!File.Exists(Path)) {
                Data = General.TUI_cfg();
                Update();
                return;
            }
            try {
                var jtoken = JsonConvert.DeserializeObject<JToken>(File.ReadAllText(Path));
                if (IsLegacy(jtoken)) {
                    UpdateLegacyFormats(jtoken);
                } else {
                    CheckSchemaErrors(jtoken);
                    Data = jtoken["Data"]!.ToObject<MainConfigData>();
                }
            } catch (JsonException ex) {
                HandleJsonParseError(ex);
            }
        }

        private void HandleJsonParseError(JsonException ex) {
            Logger.Warn("sisbase", "The provided config file is malformed!");
            Logger.Warn("sisbase", $"Details :\n    {ex.Message}");
            if (File.Exists($"{Path}.err"))
                File.Delete($"{Path}.err");
            File.Move(Path, $"{Path}.err");
            ResetAndExit();
        }

        private void CheckSchemaErrors(JToken jtoken) {
            if (!jtoken.IsValid(SchemaUtils.For<MainConfig>(), out IList<string> errors)) {
                Logger.Warn("sisbase", $"The provided config file is invalid!");
                LogSchemaErrors(errors);
                Logger.Warn("sisbase", $"Bot config was set to default values. Old config available on {Path}.backup");
                File.WriteAllText($"{Path}.backup", JsonConvert.SerializeObject(jtoken, Formatting.Indented));
                ResetAndExit();
            }
        }

        internal void UpdateLegacyFormats(JToken config) {
            Logger.Log("sisbase", "Outdated Config.json format. Updating process has begun.");
            File.WriteAllText($"{Path}.backup", JsonConvert.SerializeObject(config, Formatting.Indented));
            if (config["ConfigVersion"] == null) {  //First config. (sisbase 1.0)
                Logger.Log("sisbase", "ConfigVersion [Unknown] -> 2");
                if (!config.IsValid(SchemaUtils.For<MainConfigData>(), out IList<string> errors)) {
                    Logger.Warn("sisbase", $"Couldn't convert Config.json to Version 2");
                    LogSchemaErrors(errors);
                    Logger.Warn("sisbase", $"Bot config was set to default values. Old config available on {Path}.backup");
                    ResetAndExit();
                }
                else {
                    Data = config.ToObject<MainConfigData>();
                }
                Update();
            } else {
                Logger.Warn("sisbase", "Could not determine legacy config version");
                ResetAndExit();
            }
        }

        internal void LogSchemaErrors(IList<string> errors) {
            Logger.Warn("sisbase", $"Details :");
            foreach (var error in errors) {
                Logger.Warn("sisbase", $"    - {error}");
            }
        }

        internal void ResetAndExit() {
            Data = new MainConfigData();
            Update();
            Logger.Log("sisbase", $"Bot will now exit. Please edit the config @ {Path}");
            Environment.Exit(-1);
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
