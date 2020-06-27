﻿namespace AutoFarmer.Models.Graph
{
	public class NodeActions
	{
		public string[] InputActionNames { get; set; }

		public int AdditionalDelayBetweenActions { get; set; }

		public int AdditionalDelayAfterLastAction { get; set; }
	}
}
