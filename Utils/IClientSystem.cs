using DSharpPlus;

namespace sisbase.Utils
{
	/// <summary>
	/// Interface for <see cref="ISystem"/> that can be applied to <see cref="DiscordClient"/>
	/// </summary>
	public interface IClientSystem : ISystem
	{
		void ApplyToClient(DiscordClient client);
	}
}