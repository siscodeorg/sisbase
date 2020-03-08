using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Attributes
{
	public class PrefixAttribute : CheckBaseAttribute
	{
		public string Prefix { get; private set; }
		public PrefixAttribute(string prefix) => Prefix = prefix;
#pragma warning disable CS1998 
		public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => ctx.Prefix.ToLowerInvariant() == Prefix;
#pragma warning restore CS1998
	}
}
