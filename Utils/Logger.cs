using System;
using System.Collections.Generic;
using System.Text;
using stdout = System.Console;
using color = System.ConsoleColor;
namespace sisbase.Utils
{
	public static class Logger
	{
		public static void WriteDate()
		{
			stdout.ResetColor();
			stdout.Write($"<{DateTime.Now.ToString("MM/dd - HH:mm:ss")}> ");
		}
		public static void Log(this ISystem s, string message)
		{
			WriteDate();
			s.Status.Write();
			stdout.ForegroundColor = color.Cyan;
			stdout.Write($" {s.Name} | {message} \n");
		}
		public static void Warn(this ISystem s, string message)
		{
			WriteDate();
			s.Status.Write();
			stdout.ForegroundColor = color.Yellow;
			stdout.Write($" {s.Name} | {message} \n");
		}
		public static void Log(string source, string message)
		{
			WriteDate();
			stdout.ForegroundColor = color.Cyan;
			stdout.Write($" {source} | {message} \n");
		}
		public static void Warn(string source, string message)
		{
			WriteDate();
			stdout.ForegroundColor = color.Yellow;
			stdout.Write($" {source} | {message} \n");
		}
		public static void Write(this bool b)
		{
			stdout.ForegroundColor = b ? color.Green : color.DarkRed;
			stdout.Write($"{(b ? "🗸" : "✖")}");
		}
	}
}
