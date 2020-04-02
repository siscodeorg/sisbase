using Newtonsoft.Json;
using sisbase.Utils;
using System;
using System.IO;
using System.Linq;

namespace sisbase.Configuration
{
	/// <summary>
	/// The configuration class for the bot.
	/// </summary>
	public class Sisbase
	{
		/// <summary>
		/// Path of the configuration
		/// </summary>
		public string JsonPath { get; private set; }

		/// <summary>
		/// The configuration
		/// </summary>
		public Json Config { get; private set; }

		/// <summary>
		/// Creates a new configuration from a provided path
		/// </summary>
		/// <param name="path">The path. Must be a directory.</param>
		public Sisbase(string path)
		{
			var x = File.GetAttributes(path);
			bool isDir = x.HasFlag(FileAttributes.Directory);
			if (isDir)
			{
				bool FIRST_TIME = false;
				var di = Directory.CreateDirectory(path);
				var files = di.GetFiles().Where(x => x.Name == "Config.json").ToArray();
				if (files.Length == 0)
				{
					Config = General.TUI_cfg();
					File.WriteAllText(di.FullName + "/Config.json", JsonConvert.SerializeObject(Config, Formatting.Indented));
					FIRST_TIME = true;
				}
				else
				{
					Config = JsonConvert.DeserializeObject<Json>(File.ReadAllText(files[0].FullName));
				}
				JsonPath = FIRST_TIME ? $"{di.FullName}/Config.json" : files[0].FullName;
			}
			else
			{
				throw new InvalidOperationException("The path specified is not a Directory.");
			}
		}

		/// <summary>
		/// Updates the file of the config with the currently running config
		/// </summary>
		public void Update() => File.WriteAllText(JsonPath, JsonConvert.SerializeObject(Config, Formatting.Indented));
	}
}