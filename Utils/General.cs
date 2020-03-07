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
    }
}
