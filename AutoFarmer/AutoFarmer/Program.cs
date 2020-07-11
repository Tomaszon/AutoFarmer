﻿using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph;
using AutoFarmer.Services;
using AutoFarmer.Services.Imaging;
using AutoFarmer.Services.InputHandling;
using AutoFarmer.Services.Logging;
using AutoFarmer.Services.ReportBuilder;
using System;
using System.IO;
using System.Runtime.CompilerServices;
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
			ActionGraph graph = null;

			GraphMachine machine = null;

			try
			{
				Config.FromJsonFile(@".\configs\config.json");

				Logger.FromConfig();

				using var log = Logger.LogBlock();

				ReportBuilder.FromJsonFileWithConfig(Path.Combine(Config.Instance.ConfigDirectory, "reportBuilderConfig.json"));

				InputSimulator.FromConfig();

				MouseSafetyMeasures.FromConfig();

				ImageMatchFinder.FromConfig();

				graph = ActionGraph.FromConfig();

				machine = new GraphMachine(graph);

				Logger.GraphicalLogTemplates(ImageMatchFinder.Instance.Templates);

				Logger.Log($"Configs loaded, templates generated!");

				HandleCommand<BeforeCommand>(c =>
				{
					switch (c)
					{
						case BeforeCommand.Logs:
						{
							OpenFolder(Logger.Instance.LogDirectory);

							return true;
						}

						case BeforeCommand.Exit:
						{
							Environment.Exit(0);

							goto default;
						}

						default: return false;
					}
				});
			}
			catch (Exception ex)
			{
				throw ex;
			}

			do
			{
				try
				{
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

				HandleCommand<AfterCommand>(c =>
				{
					switch (c)
					{
						case AfterCommand.Logs:
						{
							OpenFolder(Logger.Instance.LogDirectory);

							return true;
						}

						case AfterCommand.Reports:
						{
							OpenFolder(ReportBuilder.Instance.ReportDirectory);

							return true;
						}

						case AfterCommand.Exit:
						{
							Environment.Exit(0);

							goto default;
						}

						default: return false;
					}
				});

				Logger.RefreshSessionId();

				graph.ResetStates();
			}
			while (true);
		}

		private static void HandleCommand<T>(Predicate<T> predicate) where T : struct, Enum
		{
			bool runAgain = true;

			while (runAgain)
			{
				Console.WriteLine("Choose from the following commands:");

				foreach (var value in Enum.GetValues(typeof(T)))
				{
					Console.WriteLine($"  -{value}");
				}

				var input = Console.ReadLine();

				if (!Enum.TryParse(input, true, out T command))
				{
					Console.WriteLine("Typed value was not in correct form!\n");
				}
				else
				{
					runAgain = predicate(command);
				}
			}
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

		private enum AfterCommand
		{
			Restart,
			Logs,
			Reports,
			Exit
		}

		private enum BeforeCommand
		{
			Start,
			Logs,
			Exit
		}
	}
}
