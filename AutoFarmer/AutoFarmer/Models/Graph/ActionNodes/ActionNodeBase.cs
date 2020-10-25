using AutoFarmer.Models.Common;
using System.Runtime.InteropServices.ComTypes;

namespace AutoFarmer.Models.Graph.ActionNodes
{
	public abstract class ActionNodeBase
	{
		public string[]? Actions { get; set; }

		public int AdditionalDelayBetweenActions { get; set; }

		public int AdditionalDelayAfterLastAction { get; set; } = 1000;

		public int MaxCrossing { get; set; } = 1;

		public ActionNodeFlags Flags { get; set; }

		public bool Is(ActionNodeFlags flags)
		{
			return ((int)Flags & (int)flags) == (int)flags;
		}

		public bool IsNot(ActionNodeFlags flags)
		{
			return !Is(flags);
		}

		public void AddFlags(ActionNodeFlags flags)
		{
			Flags |= flags;
		}

		public void RemoveFlags(ActionNodeFlags flags)
		{
			Flags &= ~flags;
		}
	}
}
