using System.Diagnostics;

namespace AutoFarmer.Models
{
	public class PerformanceMonitor
	{
		private readonly Stopwatch _stopwatch = new Stopwatch();

		public long Elapsed { get; private set; }

		public long Elapse
		{
			get { return _stopwatch.IsRunning ? (long)_stopwatch.Elapsed.TotalMilliseconds : Elapsed; }
		}

		public void Start()
		{
			_stopwatch.Start();
		}

		public void Stop()
		{
			_stopwatch.Stop();

			Elapsed = Elapse;

			_stopwatch.Reset();
		}
	}
}
