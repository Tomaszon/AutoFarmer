namespace AutoFarmer.Services.Extensions
{
	public static class Extensions
	{
		public static int Normalize(this int value, int minimum, int maximum)
		{
			if (value > maximum)
			{
				return maximum;
			}
			else if (value < minimum)
			{
				return minimum;
			}
			else
			{
				return value;
			}
		}
	}
}
