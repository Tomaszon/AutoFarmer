using Newtonsoft.Json;
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

			//var testSource = (Bitmap)Image.FromFile(@"C:\users\toti9\downloads\testsource.png");

			//var testSourceOutput = (Bitmap)testSource.Clone();

			//var testTemplate = imf.Templates["AssignmentDetails"];
			//Point p = imf.FindClickPointForTemplate(testSource, testTemplate.Bitmap, testTemplate.SearchRectangles["BeginAssignment"]);

			//HighlightFind(testSourceOutput, p);

			//testSourceOutput.Save(@"C:\Users\toti9\Downloads\testSourceOutput.png");

			//Console.WriteLine(p);

			//var content = JsonConvert.SerializeObject(scenario, Formatting.Indented);

			//File.WriteAllText(@"C:\Users\toti9\Downloads\out.json", content);

			//Console.WriteLine("Done");
			//Console.ReadKey();
		}

		private static void WorkMethod()
		{
			DateTime startTime = DateTime.Now;

			try
			{
				Config config = Config.FromJsonFile(@".\configs\config.json");

				List<ActionNode> actionNodes = new List<ActionNode>();
				List<ConditionEdge> conditionEdges = new List<ConditionEdge>();
				List<ImageMatchTemplate> templates = new List<ImageMatchTemplate>();
				foreach (var file in Directory.GetFiles(config.ActionNodesDirectory))
				{
					actionNodes.Add(ActionNode.FromJsonFile(file));
				}
				foreach (var file in Directory.GetFiles(config.ConditionEdgesDirectory))
				{
					conditionEdges.Add(ConditionEdge.FromJsonFile(file));
				}
				foreach (var template in Directory.GetFiles(config.ImageMatchTemplatesDirectory))
				{
					templates.Add(ImageMatchTemplate.FromJsonFile(config.ImageMatchTemplatesDirectory, config.ImageMatchTemplateResourcesDirectory));
				}

				//Scenario scenario = Scenario.FromJsonFile(Path.Combine(config.ScenarioConfigsRootDirectory, "mainScenario.json"));

				var imf = ImageMatchFinder.FromJsonFile(config.ImageMatchFinderConfigPath);
				imf.Templates = templates;

				GrafMachine machine = new GrafMachine()
				{
					ImageMatchFinder = imf,
					MouseSafetyMeasures = config.MouseSafetyMeasures
				};

				//Logger.Log($"Processing of {scenario.Name} scenario starts in {config.ProcessCountdown / 1000.0} seconds");

				Countdown(config.ProcessCountdown);

				startTime = DateTime.Now;

				//scenario.Process();

				Logger.Log($"Scenario finished in { Math.Round((DateTime.Now - startTime).TotalMinutes, 2)} minutes", NotificationType.Info);

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
