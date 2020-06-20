﻿using AutoFarmer.Models.ImageMatching;

namespace AutoFarmer.Models.GraphNamespace
{
	public class Conditions
	{
		public MatchCondition PreCondition { get; set; }

		public MatchCondition PostCondition { get; set; }

		public MatchCondition Condition { set { PreCondition = PostCondition = value; } }
	}
}
