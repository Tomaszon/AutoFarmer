using System.Collections.Generic;

namespace AutoFarmer
{
	public class GrafMachine
	{
		public MouseSafetyMeasures MouseSafetyMeasures { get; set; }

		public ImageMatchFinder ImageMatchFinder { get; set; }

		public InputSimulator InputSimulator { get; set; } = new InputSimulator();
	}
}
