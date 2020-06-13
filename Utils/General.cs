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

		internal static MainConfigData TUI_cfg()
		{
			var c = new MainConfigData();
			Console.WriteLine("Please Input the TOKEN :");
			c.Token = Console.ReadLine();
			c.MasterId = 0;
			c.PuppetId = new List<ulong>();
			c.Prefixes = new List<string>();
			Console.WriteLine("Configuration Completed.");
			return c;
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

		internal static string ToCustomName(this Type T) 
			=> $"{T.Namespace}::{T.Name}";
	}
}