namespace AutoFarmer
{
	public class Conditions
	{
		public Condition PreCondition { get; set; }

		public Condition PostCondition { get; set; }

		public Condition Condition { set { PreCondition = PostCondition = value; } }
	}
}
