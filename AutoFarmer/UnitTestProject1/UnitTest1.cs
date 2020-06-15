using System;
using System.IO;
using Adaxon.FileDirectoryService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestHashing()
		{
			string path = @"C:\users\toti9\downloads\sto sets.xlsx";

			using (var stream = new FileStream(path, FileMode.Open))
			{
				MemoryStream outStream = new MemoryStream();

				byte[] bHash = Shared.ComputeHash(stream, outStream).Result;

				string sHash = Shared.HashToString(bHash);

				byte[] currBHash = Shared.HashToBytes(sHash);

				Assert.AreEqual(bHash, currBHash);
			}
		}
	}
}
