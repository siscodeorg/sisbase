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
		/// <summary>
		/// The prefix
		/// </summary>
		public string Prefix { get; private set; }

		/// <summary>
		/// Constructs a new PrefixAttribute from a given prefix
		/// </summary>
		/// <param name="prefix">The prefix</param>
		public PrefixAttribute(string prefix) => Prefix = prefix;

#pragma warning disable CS1998, CS1591

		public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => ctx.Prefix.ToLowerInvariant() == Prefix;

#pragma warning restore CS1998
	}
}