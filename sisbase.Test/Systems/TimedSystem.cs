using sisbase.Systems;
using sisbase.Utils;
using System;
using System.Threading.Tasks;

namespace sisbase.Test.Systems
{
	public class TimedSystem : BaseSystem, IScheduledSystem
	{
		public TimeSpan Timeout { get; private set; }

		public override async Task Activate()
		{
			Name = "TimedSystem";
			Description = "A dummy System to test the scheduler";
			Status = true;
			Timeout = TimeSpan.FromMinutes(1);
		}

		public override async Task Deactivate()
		{
			Name = null;
			Description = null;
			Status = false;
		}

		public Action RunContinuous => new Action(() =>
		{
			Logger.Log(this, "Executed RunContinuous");
		});
	}
}
