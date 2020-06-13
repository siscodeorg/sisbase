using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using sisbase.Systems;
using sisbase.Utils;

namespace sisbase.Configuration {
    public class SystemConfig : IConfiguration {
        public string Path { get; set; }
        [JsonProperty] public Dictionary<string,SystemConfigData> Systems { get; set; } = new Dictionary<string, SystemConfigData>();

        public void Update() {
            File.WriteAllText(Path,JsonConvert.SerializeObject(this,Formatting.Indented));
        }
        public void Create(DirectoryInfo di) {
            Path = $"{di.FullName}/Systems.json";
            if (File.Exists(Path)) {
                var data = JsonConvert.DeserializeObject<SystemConfig>(File.ReadAllText(Path));
                Systems = data.Systems;
                return;
            }
            Flush();
            Logger.Warn("SystemConfig", "System.json was not found, default one was created.");
            Update();
        }

        internal void Flush() {
            Systems = SMC.RegisteredSystems.Select(x => new KeyValuePair<string,SystemConfigData>(
                x.Key.ToCustomName(),
                new SystemConfigData{Name = x.Key.Name})
            ).ToDictionary(kvp => kvp.Key,
                kvp => kvp.Value);
        }
    }
    
    public class SystemConfigData {
        public string Name { get; set; }
        public bool Enabled { get; set; } = true;
    }
}