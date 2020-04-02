using sisbase.Configuration;
using System;
using System.Collections.Generic;

namespace sisbase.Utils
{
	/// <summary>
	/// General utilities class
	/// </summary>
	public static class General
	{
		public static Version Version = typeof(SisbaseBot)
			.Assembly
			.GetName()
			.Version;

		public static string Format(this Version v) => $"{v.Major}.{v.Minor}.{v.Build}";

		public static string GetVersion() => Format(Version);

		public static Json TUI_cfg()
		{
			var c = new Json();
			Console.WriteLine("Please Input the TOKEN :");
			c.Token = Console.ReadLine();
			c.MasterId = 0;
			c.PuppetId = new List<ulong?>();
			c.Prefixes = new List<string>();
			Console.WriteLine("Configuration Completed.");
			return c;
		}

		public static void AddCustomConfiguration<T>(this Sisbase s, string key, T value)
		{
			s.Config.CustomSettings ??= new Dictionary<string, object>();
			s.Config.CustomSettings.TryAdd(key, value);
			s.Update();
		}

		public static void RemoveCustomConfiguration(this Sisbase s, string key)
		{
			s.Config.CustomSettings ??= new Dictionary<string, object>();
			s.Config.CustomSettings.Remove(key);
			s.Update();
		}

		public static void UpdateCustomConfiguration<T>(this Sisbase s, string key, T newValue)
		{
			s.Config.CustomSettings ??= new Dictionary<string, object>();
			s.Config.CustomSettings.TryGetValue(key, out object value);
			if (value != null) s.RemoveCustomConfiguration(key);
			s.AddCustomConfiguration<T>(key, newValue);
		}

		public static T GetCustomConfiguration<T>(this Sisbase s, string key)
		{
			s.Config.CustomSettings ??= new Dictionary<string, object>();
			s.Config.CustomSettings.TryGetValue(key, out object value);
			return (T)value;
		}
	}
}