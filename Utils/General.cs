using System;
using System.Collections.Generic;
using System.Text;

namespace sisbase.Utils
{
	public class General
	{
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
