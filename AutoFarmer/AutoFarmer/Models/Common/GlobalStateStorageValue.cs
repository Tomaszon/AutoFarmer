using System.Runtime.CompilerServices;

namespace AutoFarmer.Models.Common
{
	public class GlobalStateStorageValue
	{
		public decimal InitialValue { get; set; }

		public decimal Increment { get; set; }

		public decimal Value { get; private set; }

		public static implicit operator GlobalStateStorageValue(long value)
		{
			return new GlobalStateStorageValue() { InitialValue = value, Value = value };
		}

		public static implicit operator GlobalStateStorageValue(decimal value)
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
