﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
			DateTime startTime = DateTime.Now;

			try
			{
				Config config = Config.FromJsonFile(@".\configs\config.json");

				Graph graph = Graph.FromConfig(config);

				GraphMachine machine = new GraphMachine(config, graph);

				Logger.Log($"Processing starts in {config.ProcessCountdown / 1000.0} seconds");

				Countdown(config.ProcessCountdown);

				startTime = DateTime.Now;

				machine.Process();

				Logger.Log($"Processing finished in { Math.Round((DateTime.Now - startTime).TotalMinutes, 2)} minutes", NotificationType.Info);
			}
			catch (Exception ex)
			{
				Logger.Log(ex.Message + ":\n" + ex.ToString(), NotificationType.Error, 3);
			}

			Logger.Log("Press any key to exit");

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
				Console.Write(i.ToString());
				for (int j = 0; j < p; j++)
				{
					Console.Write(".");
					Thread.Sleep(1000 / p);
				}
			}

			Logger.Log("Processing");
		}
	}
}
