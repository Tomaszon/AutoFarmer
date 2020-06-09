//using Newtonsoft.Json;
//using System.Collections.Generic;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Threading;

//namespace AutoFarmer
//{
//	public class Scenario
//	{
//		public string Name { get; set; }

//		public string[] ScenarioNames { get; set; }

//		public static Scenario FromJsonFile(string path)
//		{
//			var scenario = JsonConvert.DeserializeObject<Scenario>(File.ReadAllText(path));
//			scenario.Name = Path.GetFileNameWithoutExtension(path);

//			return scenario;
//		}

//		public void Process()
//		{
//			InputSimulator.MoveMouse(MouseSafetyMeasures.MouseSafePosition);

//			foreach (var atomicScenario in Scenarios)
//			{
//				var screen = ScreenshotMaker.CreateScreenshot();

//				ProcessAtomicScenario(atomicScenario, screen);
//			}
//		}

//		private void ProcessAtomicScenario(ActionNode atomicScenario, Bitmap screen, int retryTimes = 0)
//		{
//			try
//			{
//				ProcessSuccessAction(atomicScenario, screen, retryTimes);	
//			}
//			catch (ImageMatchNotFoundException ex)
//			{
//				ProcessFailAction(atomicScenario, ex, screen, retryTimes);
//			}
//			catch (ImageMatchAmbiguousException ex)
//			{
//				throw new AutoFarmerException("Automatic emergency stop!", ex);
//			}
//		}

//		private void ProcessSuccessAction(ActionNode atomicScenario, Bitmap screen, int retryTimes)
//		{
//			var template = ImageMatchFinder.Templates[atomicScenario.TemplateName];

//			Logger.Log($"Processing {atomicScenario.SearchRectangleName} search rectangle of {atomicScenario.TemplateName} template for {atomicScenario.Name} atomic scenario on {retryTimes + 1}. attempt");

//			Point p = ImageMatchFinder.FindClickPointForTemplate(screen, template.Bitmap, template.SearchRectangles[atomicScenario.SearchRectangleName]);

//			if (!MouseSafetyMeasures.IsMouseInSafePosition())
//			{
//				throw new AutoFarmerException("Intentional emergency stop!");
//			}

//			InputSimulator.MoveMouse(p, atomicScenario.Actions.Success.AdditionalDelayBetweenActions);

//			InputSimulator.Simulate(atomicScenario.Actions.Success.Actions);

//			Thread.Sleep(atomicScenario.Actions.Success.AdditionalDelayAfterLastAction);

//			InputSimulator.MoveMouse(MouseSafetyMeasures.MouseSafePosition);
//		}

//		private void ProcessFailAction(ActionNode atomicScenario, ImageMatchNotFoundException ex, Bitmap screen, int retryTimes)
//		{
//			if (atomicScenario.Actions.Fail == null || retryTimes >= atomicScenario.Actions.Fail.Retry?.RetryTimes)
//			{
//				throw new AutoFarmerException("Automatic emergency stop!", ex);
//			}

//			if (atomicScenario.Actions.Fail.Retry != null)
//			{
//				Thread.Sleep(atomicScenario.Actions.Fail.Retry.DelayBeforeRetry);

//				ProcessAtomicScenario(atomicScenario, screen, ++retryTimes);
//			}
//			else
//			{
//				ProcessAtomicScenario(atomicScenario.Actions.Fail.Fallback.Scenario, screen);
//			}
//		}
//	}
//}
