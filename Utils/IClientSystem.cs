using DSharpPlus;
using System.Threading.Tasks;

namespace sisbase.Utils {
	/// <summary>
	/// Interface for <see cref="ISystem"/> that can be applied to <see cref="DiscordClient"/>
	/// </summary>
	public interface IClientSystem : ISystem {
		/// <summary>
		/// Applies the current system to the current <see cref="DiscordClient"/>
		/// </summary>
		/// <param name="client">The currrent client</param>
		Task ApplyToClient(DiscordClient client);
	}
}