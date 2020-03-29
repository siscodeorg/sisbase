using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using sisbase.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace sisbase.Attributes
{
	public class RequireSystemAttribute : CheckBaseAttribute
	{
		public ISystem System;

		public RequireSystemAttribute(Type t)
		{
			if (t.GetInterfaces().Any(x => x == typeof(ISystem)))
			{
				SMC.RegisteredSystems.TryGetValue(t, out System);
			}
		}

#pragma warning disable CS1998

		public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) => System != null;
	}
}