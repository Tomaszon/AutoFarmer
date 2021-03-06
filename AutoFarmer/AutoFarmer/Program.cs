﻿using AutoFarmer.Models.Common;
using AutoFarmer.Models.Graph;
using AutoFarmer.Services;
using AutoFarmer.Services.Imaging;
using AutoFarmer.Services.InputHandling;
using AutoFarmer.Services.Logging;
using AutoFarmer.Services.ReportBuilder;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;

namespace AutoFarmer
{
	internal class Program
	{
		private static void Main()
		{
#if DEBUG
			CopyConfigDirectory();
#else
			try
			{
#endif
			WorkMethod();

#if !DEBUG
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unexpected error in {nameof(WorkMethod)}: {ex.Message}\n{ex}");

				Console.ReadKey();
			}
#endif
		}

		private static void WorkMethod()
		{
			Configure(out ActionGraph graph, out GraphMachine machine, out string generatedConfigsDirectory);

			HandleCommand<BeforeCommand>(c =>
			{
				switch (c)
				{
					case BeforeCommand.Logs:
					{
						Open(Logger.Instance.LogDirectory);

						return true;
					}

					case BeforeCommand.Nodes:
					{
						Open(generatedConfigsDirectory);

						return true;
					}

					case BeforeCommand.Edges:
					{
						Open(generatedConfigsDirectory);

						return true;
					}

					case BeforeCommand.Reconfig:
					{
						Configure(out graph, out machine, out generatedConfigsDirectory);

						return true;
					}

					case BeforeCommand.Exit:
					{
						Environment.Exit(0);

						goto default;
					}

					case BeforeCommand.Delete:
					{
						DeleteDirectories();

						return true;
					}

					default: return false;
				}
			});

			do
			{
				DateTime startTime = DateTime.Now;

				try
				{
					Logger.Log($"Processing starts in {Config.Instance.ProcessCountdown / 1000.0} seconds", fileLog: false);

					Countdown(Config.Instance.ProcessCountdown);

					machine.Process();

					Logger.Log($"Processing finished in { Math.Round((DateTime.Now - startTime).TotalMinutes, 2)} minutes", NotificationType.Info, 5);
				}
				catch (AutoFarmerException ex)
				{
					Logger.Log(ex.Message, NotificationType.Error, 5, ex.ToString());
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
							Open(Logger.Instance.LogDirectory);

							return true;
						}

						case AfterCommand.Reports:
						{
							Open(ReportBuilder.Instance.ReportDirectory);

							return true;
						}

						case AfterCommand.Reconfig:
						{
							Configure(out graph, out machine, out generatedConfigsDirectory);

							return true;
						}

						case AfterCommand.Exit:
						{
							Environment.Exit(0);

							goto default;
						}

						case AfterCommand.Delete:
						{
							DeleteDirectories();

							return true;
						}

						default:
						{
							Console.Clear();

							return false;
						}
					}
				});

				Logger.RefreshSessionId();

				graph.ResetState(true);

				GlobalStateStorage.Reset();
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
					Console.WriteLine("Typed value was not in correct form\n");
				}
				else
				{
					runAgain = predicate(command);
				}
			}
		}

		private static void Open(string folder, string? file = null)
		{
			try
			{
				Process.Start(new ProcessStartInfo()
				{
					FileName = file is null ? folder : Path.Combine(folder, file),
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
				NotificationPlayer.Play(NotificationType.ClickSingle, i > 3 ? 1 : 2);

				for (int j = 0; j < periodCount; j++)
				{
					Console.Write(".");
					Thread.Sleep(1000 / periodCount);
				}
			}

			Console.WriteLine("\n");

			Logger.Log("Processing", NotificationType.Info);
		}

		private static void SaveToFile(string directory, string fileName, object content)
		{
			Directory.CreateDirectory(directory);

			File.WriteAllText(Path.Combine(directory, fileName), JsonConvert.SerializeObject(content, Formatting.Indented));
		}

		private static void CopyConfigDirectory()
		{
			string source = @"C:\Users\{UserName}\Documents\GitHub\AutoFarmerConfigs\Configs";

			string target = Path.Combine(Directory.GetCurrentDirectory(), "Configs");

			var userName = WindowsIdentity.GetCurrent().Name.Split('\\')[1];

			source = source.Replace("{UserName}", userName);

			DirectoryCopy(source, target);
		}

		private static void DirectoryCopy(string sourceDirName, string destDirName)
		{
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
			}

			DirectoryInfo[] dirs = dir.GetDirectories();
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}

			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files)
			{
				string temppath = Path.Combine(destDirName, file.Name);
				file.CopyTo(temppath, true);
			}

			foreach (DirectoryInfo subdir in dirs)
			{
				string temppath = Path.Combine(destDirName, subdir.Name);
				DirectoryCopy(subdir.FullName, temppath);
			}
		}

		private static void Configure(out ActionGraph graph, out GraphMachine machine, out string generatedConfigsDirectory)
		{
			Config.FromJsonFile(@".\configs\config.json");

			Logger.FromConfig();

			using var log = Logger.LogBlock();

			ReportBuilder.FromJsonFileWithConfig(Path.Combine(Config.Instance.ConfigDirectory, "reportBuilderConfig.json"));

			InputSimulator.FromConfig();

			MouseSafetyMeasures.FromConfig();

			ImageMatchFinder.FromConfig();

			GlobalStateStorage.FromConfig();

			graph = ActionGraph.FromConfig();

			machine = new GraphMachine(graph);

			generatedConfigsDirectory = Path.Combine(Directory.GetParent(Config.Instance.ConfigDirectory).FullName, "GeneratedConfigs");

			SaveToFile(generatedConfigsDirectory, "edges.json", graph.ConditionEdges.OrderBy(e => e.StartNodeName).ThenBy(e => e.EndNodeName));

			SaveToFile(generatedConfigsDirectory, "nodes.json", graph.ActionNodes.OrderBy(e => e.Name));

			Logger.GraphicalLogTemplates(ImageMatchFinder.Instance.Templates);

			Logger.Log($"Configs loaded, templates generated!");
		}

		private static void DeleteDirectories()
		{
			if (Directory.Exists(ReportBuilder.Instance.ReportDirectory))
			{
				Directory.Delete(ReportBuilder.Instance.ReportDirectory, true);
			}
			if (Directory.Exists(Logger.Instance.LogDirectory))
			{
				Directory.Delete(Logger.Instance.LogDirectory, true);
			}
		}

		private enum AfterCommand
		{
			Restart,
			Reconfig,
			Logs,
			Reports,
			Delete,
			Exit
		}

		private enum BeforeCommand
		{
			Start,
			Reconfig,
			Nodes,
			Edges,
			Logs,
			Delete,
			Exit
		}
	}
}
