using System;

namespace AutoFarmer.Models.Common
{
	public class ImageMatchNotFoundException : Exception
	{
		public ImageMatchNotFoundException(long ticks) : base($"Template not found in source image, search time: {ticks} ms.") { }
	}

	public class ImageMatchAmbiguousException : Exception
	{
		public ImageMatchAmbiguousException(int matches, long ticks) : base($"Template match is ambiguous search time: {ticks} ms. Matches: {matches}") { }
	}

	public class AutoFarmerException : Exception
	{
		public AutoFarmerException(string message) : base(message) { }

		public AutoFarmerException(string message, Exception innerException) : base(message, innerException) { }
	}
}
