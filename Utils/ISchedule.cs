using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Utils
{
	/// <summary>
	/// Interface for running scheduled functions
	/// </summary>
	public interface ISchedule : ISystem
	{
		/// <summary>
		/// Timeout for the schedule
		/// </summary>
		TimeSpan Timeout { get; }

		/// <summary>
		/// What will be run after the timeout elapses
		/// </summary>
		Action RunContinuous();
	}
}