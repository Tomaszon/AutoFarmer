using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Adaxon.FileDirectoryService
{
	public static class Shared
	{
		public static async Task<byte[]> ComputeHash(Stream inputStream, Stream outputStream)
		{
			using (var sha256 = SHA256.Create())
			using (var cryptoStream = new CryptoStream(outputStream, sha256, CryptoStreamMode.Write))
			{
				byte[] buffer = new byte[81920];
				int read;
				while ((read = await inputStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
				{
					cryptoStream.Write(buffer, 0, read);
				}
				inputStream.Close();
				cryptoStream.FlushFinalBlock();
				outputStream.Close();

				return sha256.Hash;
			}
		}

		public static string HashToString(byte[] hash)
		{
			return string.Concat(hash.Select(e => e.ToString("x2")));
		}

		public static byte[] HashToBytes(string s)
		{
			List<byte> result = new List<byte>();

			for (int index = 0; index < s.Length; index += 2)
			{
				result.Add(Convert.ToByte(s.Substring(index, 2), 16));
			}

			return result.ToArray();
		}
	}
}
