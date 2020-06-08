using System;

namespace AutoFarmer
{
	public class ImageMatchNotFoundException : Exception
	{
		public ImageMatchNotFoundException() : base("Template not found in source image!") { }
	}

	public class ImageMatchAmbiguousException : Exception
	{
		public ImageMatchAmbiguousException(int matches) : base($"Template match is ambiguous! Matches: {matches}") { }
	}

	public class AutoFarmerException : Exception
	{
		public AutoFarmerException(string message) : base(message) { }

		public AutoFarmerException(string message, Exception innerException) : base(message, innerException) { }
	}
}
