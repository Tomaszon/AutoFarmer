using System;

namespace AutoFarmer.Models.Graph
{
	public abstract class FlagBase<T> where T : struct, Enum
	{
		public T? Flags { get; set; }

		public bool Is(T flags)
		{
			return Flags is { } && (Convert.ToInt32(Flags.Value) & Convert.ToInt32(flags)) == Convert.ToInt32(flags);
		}

		public bool IsNot(T flags)
		{
			return !Is(flags);
		}

		public void AddFlags(T flags)
		{
			if (Flags is { })
			{
				Flags = (T)(object)(Convert.ToInt32(Flags.Value) | Convert.ToInt32(flags));
			}
			else
			{
				Flags = flags;
			}
		}

		public void RemoveFlags(T flags)
		{
			if (Flags is { })
			{
				Flags = (T)(object)(Convert.ToInt32(Flags.Value) & ~Convert.ToInt32(flags));

				if (Convert.ToInt32(Flags.Value) == 0)
				{
					Flags = null;
				}
			}
		}
	}
}
