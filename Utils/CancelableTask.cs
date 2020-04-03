using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace sisbase.Utils
{
	public class CancellableTask
	{
		public Task Task { get; set; }
		public CancellationTokenSource Cts { get; set; }

		public CancellableTask(Task t, CancellationTokenSource cts)
		{
			Task = t;
			Cts = cts;
		}
	}
}