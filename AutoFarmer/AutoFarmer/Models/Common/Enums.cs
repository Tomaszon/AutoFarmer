using System;

namespace AutoFarmer.Models
{
	public enum NotificationType
	{
		None,
		Click,
		Error,
		Info
	}

	public enum MouseAction
	{
		LeftClick,
		LeftDoubleClick,
		ScanScreenForFadedUI,
		LeftHold1sec = 1000,
		LeftHold5sec = 5000
	}

	public enum MatchOrderBy
	{
		X,
		Y
	}

	public enum MatchOrderLike
	{
		Ascending,
		Descending
	}

	public enum NamedSearchArea
	{
		Full = 0b1111,
		Left = 0b1101,
		Right = 0b0101,
		Upper = 0b1110,
		Lower = 0b1010,
		UpperLeft = 0b1100,
		UpperRight = 0b0100,
		LowerLeft = 0b1000,
		LowerRight = 0b0000
	}
}
