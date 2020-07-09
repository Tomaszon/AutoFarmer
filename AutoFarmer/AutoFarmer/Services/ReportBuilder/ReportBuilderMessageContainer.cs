using AutoFarmer.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoFarmer.Services.ReportBuilder
{
	public class ReportBuilderMessageContainer
	{
		public Dictionary<string, List<string>> Messages { get; set; } = new Dictionary<string, List<string>>();

		private readonly Dictionary<Tuple<string, ReportMessageType>, List<string>> _buffer = new Dictionary<Tuple<string, ReportMessageType>, List<string>>();

		public void AddToMessages(string key, string value)
		{
			if (Messages.ContainsKey(key))
			{
				Messages[key].Add(value);
			}
			else
			{
				Messages.Add(key, new List<string>(new[] { value }));
			}
		}

		public void AddToBuffer(string key, ReportMessageType type, string value)
		{
			var k = new Tuple<string, ReportMessageType>(key, type);

			if (_buffer.ContainsKey(k))
			{
				_buffer[k].Add(value);
			}
			else
			{
				_buffer.Add(k, new List<string>(new[] { value }));
			}
		}

		public void Commit(ReportMessageType type)
		{
			_buffer.Where(t =>
				t.Key.Item2 == type).Select(t =>
					new KeyValuePair<string, List<string>>(t.Key.Item1, t.Value)).ToList().ForEach(t =>
						Messages.Add(t.Key, t.Value));

			ClearBuffer();
		}

		public void Clear()
		{
			Messages.Clear();
		}

		public void ClearBuffer()
		{
			_buffer.Clear();
		}
	}
}
