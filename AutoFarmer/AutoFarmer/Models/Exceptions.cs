using System;

namespace AutoFarmer.Models
{
	public class ImageMatchNotFoundException : Exception
	{
		public ImageMatchNotFoundException(long ticks) : base($"Template not found in source image! Search time: {ticks} ms.") { }
	}

	public class ImageMatchAmbiguousException : Exception
	{
		public ImageMatchAmbiguousException(int matches, long ticks) : base($"Template match is ambiguous! Search time: {ticks} ms. Matches: {matches}") { }
	}

	public class AutoFarmerException : Exception
	{
		public AutoFarmerException(string message) : base(message) { }

		public AutoFarmerException(string message, Exception innerException) : base(message, innerException) { }
	}
}
