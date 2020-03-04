namespace sisbase.Utils
{
	/// <summary>
	/// If this is not implemented by a system, it will not be available for use.
	/// Interface for systems that don't require connection to <see cref="DSharpPlus"/>
	/// </summary>
	public interface IStaticSystem : ISystem
	{
	}
}