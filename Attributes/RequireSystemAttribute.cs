using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using sisbase.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;
using sisbase.Systems;

namespace sisbase.Attributes
{
	/// <summary>
	/// Attribute that checks if an specified system is registered on the <see cref="SMC"/>.<br></br>
	/// If the system exists the command is executed.
	/// </summary>
	public class RequireSystemAttribute : CheckBaseAttribute
	{
		/// <summary>
		/// The System
		/// </summary>
		public ISystem System;

		/// <summary>
		/// Constructs a new RequireSystemAttribute from a given system type.
		/// </summary>
		/// <param name="t">The system type <br></br>
		/// Must inherits <see cref="ISystem"/>
		/// </param>
		public RequireSystemAttribute(Type t)
		{
			if (t.GetInterfaces().Any(x => x == typeof(ISystem)))
			{
				SMC.RegisteredSystems.TryGetValue(t, out System);
			}
		}

#pragma warning disable CS1998, CS1591

		public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => System != null;
	}
}