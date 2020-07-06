using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph;
using AutoFarmer.Models.ImageMatching;
using AutoFarmer.Models.InputHandling;
using System;
using System.Threading;

namespace AutoFarmer
{
	internal class Program
	{
		private static void Main()
		{
			Config.FromJsonFile(@".\configs\config.json");

			Logger.FromConfig();

			WorkMethod();
		}

		private static void WorkMethod()
		{
			using var log = Logger.LogBlock();

			string input;

			do
			{
				try
				{
					InputSimulator.FromConfig();

					MouseSafetyMeasures.FromConfig();

					ImageMatchFinder.FromConfig();

					ActionGraph graph = ActionGraph.FromConfig();

					GraphMachine machine = new GraphMachine(graph);

					Logger.Log($"Processing starts in {Config.Instance.ProcessCountdown / 1000.0} seconds", fileLog: false);

					Countdown(Config.Instance.ProcessCountdown);

					DateTime startTime = DateTime.Now;

					machine.Process();

					Thread.Sleep(1000);

					Logger.Log($"Processing finished in { Math.Round((DateTime.Now - startTime).TotalMinutes, 2)} minutes", NotificationType.Info);
				}
				catch (AutoFarmerException ex)
				{
					Logger.Log(ex.Message + ex.ToString(), NotificationType.Error, 3);
				}

				Logger.Log("Press 'y' for restart. Others will exit.", fileLog: false);

				input = Console.ReadLine();
			}
			while (input == "y");
		}

		private static void Countdown(int milliseconds)
		{
			int periodCount = 3;
			int seconds = milliseconds / 1000;
			int fraction = milliseconds % 1000;

			Thread.Sleep(fraction);

			for (int i = seconds; i > 0; i--)
			{
				Console.Write(i.ToString());
				
				for (int j = 0; j < periodCount; j++)
				{
					Console.Write(".");
					Thread.Sleep(1000 / periodCount);
				}
			}

			Console.WriteLine("\n");

			Logger.Log("Processing", NotificationType.Info);
		}
	}
}
