using AutoFarmer.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoFarmer.Models.Graph
{
	public interface IOptions
	{
		protected static string ReplaceVariables(string value, Dictionary<string, List<object>> varables, int index)
		{
			foreach (var t in varables)
			{
				value = value.Replace($"{{{t.Key}}}", t.Value[index].ToString());
			}

			return value;
		}

		protected static bool IsContainVariable(List<string> parameterNames, params string[] values)
		{
			return parameterNames.Any(p => values.Any(a => a.Contains($"{{{p}}}")));
		}

		protected static List<T> FromJsonFileWrapper<T>(Func<List<T>> func)
		{
			try
			{
				return func.Invoke();
			}
			catch (Exception ex)
			{
				throw new AutoFarmerException("Configuration error!", ex);
			}
		}
	}
}
