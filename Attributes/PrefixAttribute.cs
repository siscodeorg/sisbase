using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace sisbase.Attributes
{
	/// <summary>
	/// Attribute that checks if a command was executed with an specified prefix
	/// </summary>
	public class PrefixAttribute : CheckBaseAttribute
	{
		public string Prefix { get; private set; }

		public PrefixAttribute(string prefix) => Prefix = prefix;

#pragma warning disable CS1998

		public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => ctx.Prefix.ToLowerInvariant() == Prefix;

#pragma warning restore CS1998
	}
}