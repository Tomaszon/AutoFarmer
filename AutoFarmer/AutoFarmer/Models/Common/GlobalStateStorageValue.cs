using System.Runtime.CompilerServices;

namespace AutoFarmer.Models.Common
{
	public class GlobalStateStorageValue
	{
		public long InitialValue { get; set; }

		public long Increment { get; set; }

		public long Value { get; private set; }

		public static implicit operator GlobalStateStorageValue(long value)
		{
			return new GlobalStateStorageValue() { InitialValue = value, Value = value };
		}

		public void Increase()
		{
			Value += Increment;
		}

		public void Reset()
		{
			Value = InitialValue;
		}
	}
}
