using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;

namespace AutoFarmer
{
	public class Scenario
	{
		public string Name { get; set; }

		public string[] ScenarioNames { get; set; }

		public MouseSafetyMeasures MouseSafetyMeasures { get; set; }

		public ImageMatchFinder ImageMatchFinder { get; set; }

		public InputSimulator InputSimulator { get; set; }

		public List<AtomicScenario> Scenarios { get; } = new List<AtomicScenario>();

		public static Scenario FromJsonFile(string path)
		{
			var scenario = JsonConvert.DeserializeObject<Scenario>(File.ReadAllText(path));
			scenario.Name = Path.GetFileNameWithoutExtension(path);

			scenario.InputSimulator = new InputSimulator();

			return scenario;
		}

		public static List<Scenario> LoadScenarios(string directory)
		{
			List<Scenario> result = new List<Scenario>();

			if (Directory.Exists(directory))
			{
				foreach (string fileName in Directory.GetFiles(directory))
				{
					result.Add(FromJsonFile(fileName));
				}
			}
			return result;
		}

		public void Init(List<Scenario> mainGroupScenarios, List<AtomicScenario> mainAtomicScenarios, List<AtomicScenario> mainScenarios = null)
		{
			foreach (var name in ScenarioNames)
			{
				var group = mainGroupScenarios.FirstOrDefault(s => s.Name == name);
				if (group != null)
				{
					group.Init(mainGroupScenarios, mainAtomicScenarios, mainScenarios ?? Scenarios);
				}
				else
				{
					(mainScenarios ?? Scenarios).Add(mainAtomicScenarios.First(s => s.Name == name));
				}
			}
		}

		public void Process()
		{
			InputSimulator.MoveMouse(MouseSafetyMeasures.MouseSafePosition);

			foreach (var atomicScenario in Scenarios)
			{
				var screen = ScreenshotMaker.CreateScreenshot();

				ProcessAtomicScenario(atomicScenario, screen);
			}
		}

		private void ProcessAtomicScenario(AtomicScenario atomicScenario, Bitmap screen, int retryTimes = 0)
		{
			try
			{
				var template = ImageMatchFinder.Templates[atomicScenario.TemplateName];

				Logger.Log($"Processing {atomicScenario.SearchRectangleName} search rectangle of {atomicScenario.TemplateName} template for {atomicScenario.Name} atomic scenario on {retryTimes+1}. attempt");

				Point p = ImageMatchFinder.FindClickPointForTemplate(screen, template.Bitmap, template.SearchRectangles[atomicScenario.SearchRectangleName]);

				if (!MouseSafetyMeasures.IsMouseInSafePosition())
				{
					throw new AutoFarmerException("Intentional emergency stop!");
				}

				InputSimulator.MoveMouse(p, atomicScenario.Actions.Success.AdditionalDelayBetweenActions);

				InputSimulator.Simulate(atomicScenario.Actions.Success.Actions);

				Thread.Sleep(atomicScenario.Actions.Success.AdditionalDelayAfterLastAction);

				InputSimulator.MoveMouse(MouseSafetyMeasures.MouseSafePosition);
			}
			catch (ImageMatchNotFoundException ex)
			{
				if (atomicScenario.Actions.Fail == null || retryTimes >= atomicScenario.Actions.Fail.Retry?.RetryTimes)
				{
					throw new AutoFarmerException("Automatic emergency stop!", ex);
				}

				if (atomicScenario.Actions.Fail.Retry != null)
				{
					Thread.Sleep(atomicScenario.Actions.Fail.Retry.DelayBeforeRetry);

					ProcessAtomicScenario(atomicScenario, screen, ++retryTimes);
				}
				else
				{
					ProcessAtomicScenario(atomicScenario.Actions.Fail.Fallback.Scenario, screen);
				}
			}
			catch (ImageMatchAmbiguousException ex)
			{
				throw new AutoFarmerException("Automatic emergency stop!", ex);
			}
		}
	}
}
