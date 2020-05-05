using sisbase.Attributes;
using System;
using color = System.ConsoleColor;
using stdout = System.Console;

namespace sisbase.Utils
{
	/// <summary>
	/// The Logger
	/// </summary>
	public static class Logger
	{
		private static void WriteDate()
		{
			stdout.ResetColor();
			stdout.Write($"\n <{DateTime.Now:MM/dd - HH:mm:ss}> ");
		}

		/// <summary>
		/// Logs to the console from a system. Level : Log
		/// </summary>
		/// <param name="s">The system , usually used as an extension method</param>
		/// <param name="message">The message that would be logged</param>
		public static void Log(this ISystem s, string message)
		{
			WriteDate();
			s.Status.Write();
			stdout.ForegroundColor = s.IsVital() ? color.Yellow : color.Cyan;
			stdout.Write($"{s.Name} ");
			stdout.ForegroundColor = color.Cyan;
			stdout.Write($"| {message} \t");
			stdout.ResetColor();
		}

		/// <summary>
		/// Logs to the console from a system. Level : Warning
		/// </summary>
		/// <param name="s">The system , usually used as an extension method</param>
		/// <param name="message">The message that would be logged</param>
		public static void Warn(this ISystem s, string message)
		{
			WriteDate();
			s.Status.Write();
			stdout.ForegroundColor = color.Yellow;
			stdout.Write($" {s.Name} | {message} \t");
			stdout.ResetColor();
		}

		/// <summary>
		/// Logs to the console from a system. Level : Log
		/// </summary>
		/// <param name="source">The source from where the log is called</param>
		/// <param name="message">The message that would be logged</param>
		public static void Log(string source, string message)
		{
			WriteDate();
			stdout.ForegroundColor = color.Cyan;
			stdout.Write($" {source} | {message} \t");
			stdout.ResetColor();
		}

		/// <summary>
		/// Logs to the console from a system. Level : Warn
		/// </summary>
		/// <param name="source">The source from where the log is called</param>
		/// <param name="message">The message that would be logged</param>
		public static void Warn(string source, string message)
		{
			WriteDate();
			stdout.ForegroundColor = color.Yellow;
			stdout.Write($" {source} | {message} \t");
			stdout.ResetColor();
		}

		/// <summary>
		/// Simple utility to log booleans nicely
		/// </summary>
		/// <param name="b">The boolean</param>
		public static void Write(this bool b)
		{
			stdout.ForegroundColor = b ? color.Green : color.DarkRed;
			stdout.Write($"{(b ? "O" : "X")} ");
			stdout.ResetColor();
		}
	}
}