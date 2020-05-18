using sisbase.Utils;
using System;

namespace sisbase.Test.Systems {
	public class TimedSystem : IScheduledSystem {
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Status { get; set; }

		public TimeSpan Timeout { get; private set; }

		public void Activate() {
			Name = "TimedSystem";
			Description = "A dummy System to test the scheduler";
			Status = true;
			Timeout = TimeSpan.FromMinutes(1);
		}

		public void Deactivate() {
			Name = null;
			Description = null;
			Status = false;
		}

		public void Execute() => Logger.Log(this, "Command Executed from Execute()");

		public Action RunContinuous => new Action(() => {
			Logger.Log(this, "Executed RunContinuous");
		});
	}
}