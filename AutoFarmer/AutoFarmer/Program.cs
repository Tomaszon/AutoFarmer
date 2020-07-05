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

			WorkMethod();
		}

		private static void WorkMethod()
		{
			string input;

			do
			{
				Logger.Log("alma");
				Logger.Log("körte");

				using var log = Logger.BlockLog();

				try
				{
					InputSimulator.FromConfig();

					MouseSafetyMeasures.FromConfig();

					ImageMatchFinder.FromConfig();

					ActionGraph graph = ActionGraph.FromConfig();

					GraphMachine machine = new GraphMachine(graph);

					//Console.WriteLine($"Processing starts in {Config.Instance.ProcessCountdown / 1000.0} seconds\n");

					//Countdown(Config.Instance.ProcessCountdown);

					DateTime startTime = DateTime.Now;

					//machine.Process();

					Thread.Sleep(2000);

					//Logger.Log($"Processing finished in { Math.Round((DateTime.Now - startTime).TotalMinutes, 2)} minutes", NotificationType.Info);
				}
				catch (AutoFarmerException ex)
				{
					Logger.Log(ex.Message + ":\n" + ex.ToString(), NotificationType.Error, 3);
				}

				//Console.WriteLine("Press 'y' for restart. Others will exit.\n");

				Logger.Log("oooooooooooooooooooooooooooooooooooooooooooooooooo");
				Logger.Log("aooooooooooooooooooooooooooooooooooooooooooooa");
				Logger.Log("aooooooooooooooooooooooooooooooooooooooooooooooooa aooooooooa aooooooooooooooooooa aa aoooooooooooooooooooooooooa aooooooooooooooooooa aooooooooooooooooooooooooooooa aooooooooooooooooooooooooooooooooooooooa");
				Logger.Log("aoooooooooooooooooooooooooooooooooooooooooa aooooooooooooooa aoooooooooooooooooooooooooooooooooooooa");
				using var log2 = Logger.BlockLog();
				Logger.Log("aooooooooooooooooooooooooooooooooooooooooooooooooooo.ooooooooooooooooooooooooooooooooooooooooooo.ooooooooooooooooooooooooooooooooooooooooa");
				Logger.Log("aooooooooa aooooooooa aooooooooa aooooooooooa aooooooooa aooooooooa aooooooooa aooooooooooa");
				Logger.Log("oo                                                                                             oooooooooooooooooooooooooooooooooooooooooooooooo");


				input = Console.ReadLine();
			}
			while (input == "y");

			Console.WriteLine("Press any button to exit!");
			Console.ReadKey();
		}

		private static void Countdown(int milliseconds)
		{
			int p = 3;
			int seconds = milliseconds / 1000;
			int fraction = milliseconds % 1000;

			Thread.Sleep(fraction);

			for (int i = seconds; i > 0; i--)
			{
				//Console.Write(i.ToString());
				for (int j = 0; j < p; j++)
				{
					//Console.Write(".");
					Thread.Sleep(1000 / p);
				}
			}

			//Console.WriteLine("\n");

			Logger.Log("Processing", NotificationType.Info);
		}
	}
}
