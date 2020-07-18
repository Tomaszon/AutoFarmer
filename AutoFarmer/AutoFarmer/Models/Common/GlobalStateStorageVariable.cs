namespace AutoFarmer.Models.Common
{
	public class GlobalStateStorageVariable
	{
		public int DefaultValue { get; set; }

		public int Increment { get; set; }

		public int Value { get; private set; }

		public void Increase()
		{
			Value += Increment;
		}

		public void Reset()
		{
			Value = DefaultValue;
		}
	}
}
