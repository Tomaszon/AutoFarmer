namespace AutoFarmer
{
	public class Conditions
	{
		public ImageFindCondition PreCondition { get; set; }

		public ImageFindCondition PostCondition { get; set; }

		public ImageFindCondition Condition { set { PreCondition = PostCondition = value; } }
	}
}
