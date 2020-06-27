using AutoFarmer.Models;
using AutoFarmer.Models.Graph;
using AutoFarmer.Models.ImageMatching;
using System;
using System.Threading;

namespace AutoFarmer
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			WorkMethod();
		}

		private static void WorkMethod()
		{
			string input;

			do
			{
				try
				{
					Config.FromJsonFile(@".\configs\config.json");

					Console.Clear();

					MouseSafetyMeasures.FromConfig();

					ImageMatchFinder.FromConfig();

					WorkflowGraph graph = WorkflowGraph.FromConfig();

					GraphMachine machine = new GraphMachine(graph);

					Console.WriteLine($"Processing starts in {Config.Instance.ProcessCountdown / 1000.0} seconds\n");

					Countdown(Config.Instance.ProcessCountdown);

					DateTime startTime = DateTime.Now;

					machine.Process();

					Logger.Log($"Processing finished in { Math.Round((DateTime.Now - startTime).TotalMinutes, 2)} minutes", NotificationType.Info);
				}
				catch (AutoFarmerException ex)
				{
					Logger.Log(ex.Message + ":\n" + ex.ToString(), NotificationType.Error, 3);
				}

				Console.WriteLine("Press 'y' for restart. Others will exit.\n");

				Logger.Log("---------------------------------------------------------------------------" +
						   "---------------------------------------------------------------------------");

				input = Console.ReadLine();
			}
			while (input == "y");
		}

		private static void Countdown(int milliseconds)
		{
			int p = 3;
			int seconds = milliseconds / 1000;
			int fraction = milliseconds % 1000;

			Thread.Sleep(fraction);

			for (int i = seconds; i > 0; i--)
			{
				Console.Write(i.ToString());
				for (int j = 0; j < p; j++)
				{
					Console.Write(".");
					Thread.Sleep(1000 / p);
				}
			}

			Console.WriteLine("\n");

			Logger.Log("Processing", NotificationType.Info);
		}
	}
}
