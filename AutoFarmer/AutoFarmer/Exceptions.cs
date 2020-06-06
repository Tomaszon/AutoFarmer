using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace image_search_test
{
	public class ImageMatchNotFoundException : Exception
	{
		public ImageMatchNotFoundException() : base("Template not found in source image!") { }
	}

	public class ImageMatchAmbiguousException : Exception
	{
		public ImageMatchAmbiguousException(int matches) : base($"Template match is ambiguous! Matches: {matches}") { }
	}
}
