using System;

namespace sisbase.Systems
{
	/// <summary>
	/// Interface for running a scheduled function on systems
	/// </summary>

	public interface IScheduledSystem : ISystem
	{
		/// <summary>
		/// Timeout for the schedule
		/// </summary>
		TimeSpan Timeout { get; }

		/// <summary>
		/// What will be run after the timeout elapses
		/// </summary>
		Action RunContinuous { get; }
	}
}