using AutoFarmer.Services.Logging;
using System.Diagnostics;

namespace AutoFarmer.Services
{
	public class PerformanceMonitor
	{
		private readonly Stopwatch _stopwatch = new Stopwatch();

		public long Elapsed { get; private set; }

		public long Elapse
		{
			get { return _stopwatch.IsRunning ? _stopwatch.ElapsedMilliseconds : Elapsed; }
		}

		public void Start()
		{
			using var log = Logger.LogBlock();

			_stopwatch.Start();
		}

		public void Stop()
		{
			using var log = Logger.LogBlock();

			_stopwatch.Stop();

			Elapsed = _stopwatch.ElapsedMilliseconds;

			_stopwatch.Reset();
		}
	}
}
