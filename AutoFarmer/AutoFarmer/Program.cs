using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph;
using AutoFarmer.Services;
using AutoFarmer.Services.Imaging;
using AutoFarmer.Services.InputHandling;
using AutoFarmer.Services.Logging;
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
#if !DEBUG
			try
			{
#endif
			WorkMethod();

#if !DEBUG
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unexpected error in {nameof(WorkMethod)}: {ex.Message}\n{ex}");
			}
#endif
		}

		private static void WorkMethod()
		{
			do
			{
				try
				{
					Config.FromJsonFile(@".\configs\config.json");

					Logger.FromConfig();

					using var log = Logger.LogBlock();

					ReportBuilder.FromJsonFileWithConfig(Path.Combine(Config.Instance.ConfigDirectory, "reportBuilderConfig.json"));

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
				finally
				{
					try
					{
						ReportBuilder.Generate();
					}
					catch (Exception ex)
					{
						Console.WriteLine($"{ex}\n");
					}
				}

			CommandSelection:
				Command command;

				Console.WriteLine("Choose from the following commands:");
				foreach (var value in Enum.GetValues(typeof(Command)))
				{
					Console.WriteLine($"  -{value}");
				}

				var input = Console.ReadLine();

				if (!Enum.TryParse(input, true, out command))
				{
					Console.WriteLine("Typed value was not in correct form!\n");

					goto CommandSelection;
				}

				switch (command)
				{
					case Command.Logs:
					{
						OpenFolder(Logger.Instance.LogDirectory);
						goto CommandSelection;
					}
					case Command.Reports:
					{
						OpenFolder(ReportBuilder.Instance.ReportDirectory);
						goto CommandSelection;
					}
					case Command.Exit:
						return;
				}

				Logger.RefreshSessionId();
			}
			while (true);
		}

		private static void OpenFolder(string folder)
		{
			try
			{
				System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
				{
					FileName = folder,
					UseShellExecute = true,
					Verb = "open"
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{ex}\n");
			}
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

		private enum Command
		{
			Restart,
			Logs,
			Reports,
			Exit
		}
	}
}
