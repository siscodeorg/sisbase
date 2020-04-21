using System.Threading.Tasks;

namespace sisbase.Test
{
	internal class Program
	{
		private static async Task Main()
		{
			var sisbase = new SisbaseBot();
			sisbase.RegisterBot(typeof(Program).Assembly);
			await sisbase.Start();
		}
	}
}