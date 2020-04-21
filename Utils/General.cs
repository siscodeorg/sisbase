using DSharpPlus.Entities;
using sisbase.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace sisbase.Utils
{
	/// <summary>
	/// General utilities class
	/// </summary>
	public static class General
	{
		internal static Version Version = typeof(SisbaseBot)
			.Assembly
			.GetName()
			.Version;

		internal static string Format(this Version v) => $"{v.Major}.{v.Minor}.{v.Build}";

		internal static string GetVersion() => Format(Version);

		internal static Json TUI_cfg()
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

		/// <summary>
		/// Adds a new custom configuration onto the bot config.
		/// </summary>
		/// <typeparam name="T"> Type of the custom configuration</typeparam>
		/// <param name="s"></param>
		/// <param name="key">The name of the custom configuration</param>
		/// <param name="value"></param>
		public static void AddCustomConfiguration<T>(this Sisbase s, string key, T value)
		{
			s.Config.CustomSettings ??= new Dictionary<string, object>();
			s.Config.CustomSettings.TryAdd(key, value);
			s.Update();
		}

		/// <summary>
		/// Removes an existing custom configuration from the bot config.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="key">The name of the custom configuration</param>
		public static void RemoveCustomConfiguration(this Sisbase s, string key)
		{
			s.Config.CustomSettings ??= new Dictionary<string, object>();
			s.Config.CustomSettings.Remove(key);
			s.Update();
		}

		/// <summary>
		/// Updates an existing custom configuration from the bot config.
		/// </summary>
		/// <typeparam name="T">Type of the custom configuration</typeparam>
		/// <param name="s"></param>
		/// <param name="key">The name of the custom configuration</param>
		/// <param name="newValue">The updated value</param>
		public static void UpdateCustomConfiguration<T>(this Sisbase s, string key, T newValue)
		{
			s.Config.CustomSettings ??= new Dictionary<string, object>();
			s.Config.CustomSettings.TryGetValue(key, out object value);
			if (value != null) s.RemoveCustomConfiguration(key);
			s.AddCustomConfiguration<T>(key, newValue);
		}

		/// <summary>
		/// Gets the value of an existing custom configuration from the bot config.
		/// </summary>
		/// <typeparam name="T"> Type of the custom configuration</typeparam>
		/// <param name="s"></param>
		/// <param name="key">The name of the custom configuration</param>
		public static T GetCustomConfiguration<T>(this Sisbase s, string key)
		{
			s.Config.CustomSettings ??= new Dictionary<string, object>();
			s.Config.CustomSettings.TryGetValue(key, out object value);
			return (T)value;
		}

		/// <summary>
		/// Returns the first integer value found on a <see cref="DiscordMessage"/>
		/// </summary>
		/// <param name="m">The message</param>
		/// <returns></returns>
		public static int FirstInt(this DiscordMessage m) =>
			int.Parse(m.Content.Split(" ").Where(x => int.TryParse(x, out int _)).FirstOrDefault());

		/// <summary>
		/// Returns the first emoji found on a <see cref="DiscordMessage"/>
		/// </summary>
		/// <param name="m">The message</param>
		/// <returns></returns>
		public static DiscordEmoji FirstEmoji(this DiscordMessage m)
		{
			string str = m.Content.Split(" ").Where(x => Emoji.EmojiLiterals.Contains(x) || Regex.IsMatch(x, @"\<\:([a-zA-Z_0-9]+)\:([0-9]+)\>|\<\:([a-zA-Z_0-9]+)\:\>")).FirstOrDefault();
			if (string.IsNullOrEmpty(str)) return null;
			if (Emoji.EmojiLiterals.Contains(str)) return DiscordEmoji.FromUnicode(str);
			string customEmoji = Regex.Match(str, @"\:([a-zA-Z_0-9]+)\:").Value;
			return DiscordEmoji.FromName(SisbaseBot.Instance.Client, customEmoji);
		}
	}
}