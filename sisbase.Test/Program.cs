using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using sisbase;
using sisbase.Configuration;

namespace sisbase.Test
{
	class Program
	{
		private static CancellationTokenSource cts { get; } = new CancellationTokenSource();
		private static async Task Main()
		{
			Console.CancelKeyPress += (sender, e) =>
			{
				if (!cts.IsCancellationRequested)
					cts.Cancel();
				e.Cancel = true;
			};

			var sisbase = new SisbaseBot(
				new Sisbase(Directory.GetCurrentDirectory())
			);

			await sisbase.StartAsync();
			while (!cts.IsCancellationRequested)
				await Task.Delay(1);
		}
	}
}
