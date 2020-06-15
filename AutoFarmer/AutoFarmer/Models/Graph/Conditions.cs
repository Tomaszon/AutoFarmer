using AutoFarmer.Models.ImageMatching;

namespace AutoFarmer.Models.GraphNamespace
{
	public class Conditions
	{
		public FindCondition PreCondition { get; set; }

		public FindCondition PostCondition { get; set; }

		public FindCondition Condition { set { PreCondition = PostCondition = value; } }
	}
}
