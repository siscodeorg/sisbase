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

        /// <summary>
        /// Adds a new custom configuration onto the bot config.
        /// </summary>
        /// <typeparam name="T">Type of the custom configuration</typeparam>
        /// <param name="s"></param>
        /// <param name="key">The name of the custom configuration</param>
        /// <param name="value"></param>
        public void AddCustomConfiguration<T>(string key, T value) {
            Data.CustomSettings ??= new Dictionary<string, object>();
            Data.CustomSettings.TryAdd(key, value);
            Update();
        }

        /// <summary>
        /// Removes an existing custom configuration from the bot config.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="key">The name of the custom configuration</param>
        public void RemoveCustomConfiguration(string key) {
            Data.CustomSettings ??= new Dictionary<string, object>();
            Data.CustomSettings.Remove(key);
            Update();
        }

        /// <summary>
        /// Updates an existing custom configuration from the bot config.
        /// </summary>
        /// <typeparam name="T">Type of the custom configuration</typeparam>
        /// <param name="s"></param>
        /// <param name="key">The name of the custom configuration</param>
        /// <param name="newValue">The updated value</param>
        public void UpdateCustomConfiguration<T>(string key, T newValue) {
            Data.CustomSettings ??= new Dictionary<string, object>();
            Data.CustomSettings.TryGetValue(key, out object value);
            if (value != null) RemoveCustomConfiguration(key);
            AddCustomConfiguration(key, newValue);
        }

        /// <summary>
        /// Gets the value of an existing custom configuration from the bot config.
        /// </summary>
        /// <typeparam name="T">Type of the custom configuration</typeparam>
        /// <param name="s"></param>
        /// <param name="key">The name of the custom configuration</param>
        public T GetCustomConfiguration<T>(string key) {
            Data.CustomSettings ??= new Dictionary<string, object>();
            Data.CustomSettings.TryGetValue(key, out object value);
            return (T) value;
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
