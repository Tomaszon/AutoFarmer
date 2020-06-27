using System.Collections.Generic;
using System.Linq;

namespace AutoFarmer.Models.Graph
{
	public abstract class Options
	{
		protected static string ReplaceTemplates(string value, Dictionary<string, List<object>> parameters, int index)
		{
			foreach (var t in parameters)
			{
				value = value.Replace($"{{{t.Key}}}", t.Value[index].ToString());
			}

			return value;
		}

		protected static bool IsContainsTemplate(List<string> parameterNames, params string[] values)
		{
			return parameterNames.Any(p => values.Any(a => a.Contains($"{{{p}}}")));
		}
	}
}
