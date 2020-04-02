using sisbase.Configuration;
using sisbase.Utils;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace sisbase.Test
{
	internal class Program
	{
		private static CancellationTokenSource Cts { get; } = new CancellationTokenSource();

		private static async Task Main()
		{
			Console.CancelKeyPress += (sender, e) =>
			{
				if (!Cts.IsCancellationRequested)
					Cts.Cancel();
				e.Cancel = true;
			};
			var config = new Sisbase(Directory.GetCurrentDirectory());

			config.AddCustomConfiguration<Json>("another config", new Json());
			var sisbase = new SisbaseBot(
				config
			);
			sisbase.RegisterBot(typeof(Program).Assembly);
			await sisbase.StartAsync();
			while (!Cts.IsCancellationRequested)
				await Task.Delay(1);
		}
	}
}