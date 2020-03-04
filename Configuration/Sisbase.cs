using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using sisbase.Utils;
namespace sisbase.Configuration
{
	public class Sisbase
	{
		public string JsonPath { get; private set; }
		public Json Config { get; private set; }

		public Sisbase(string path)
		{
			var x = File.GetAttributes(path);
			bool isDir = x.HasFlag(FileAttributes.Directory);
			if(isDir)
			{
				var di = Directory.CreateDirectory(path);
				var files = di.GetFiles().Where(x => x.Name == "Config.json").ToArray();
				if(files.Length == 0)
				{
					Config = General.TUI_cfg();
					File.WriteAllText(di.FullName + "/Config.json", JsonConvert.SerializeObject(Config, Formatting.Indented));
				}
				else
				{
					Config = JsonConvert.DeserializeObject<Json>(File.ReadAllText(files[0].FullName)); 
				}
				JsonPath = files[0].FullName ?? $"{di.FullName}/Config.json";
			}
			else
			{
				throw new InvalidOperationException("The path specified is not a Directory.");
			}
		}
	}
}
