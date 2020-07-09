using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph;
using AutoFarmer.Services;
using AutoFarmer.Services.Imaging;
using AutoFarmer.Services.InputHandling;
using AutoFarmer.Services.ReportBuilder;
using System;
using System.IO;
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
			string input;

			do
			{
				try
				{
					using var log = Logger.LogBlock();

					ReportBuilder.FromJsonFileWithConfig(Path.Combine(Config.Instance.ConfigDirectory, "reportBuilderConfig.json"));

					ReportBuilder.Add("dummy", ReportMessageType.Success,"message");
					ReportBuilder.Add("dummy", ReportMessageType.Success, "message2");
					ReportBuilder.Add("dummy2", ReportMessageType.Success, "message");

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

				ReportBuilder.Generate();

				Logger.Log("Press 'y' for restart. Others will exit.", fileLog: false);

				input = Console.ReadLine();

				Logger.RefreshSessionId();
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
