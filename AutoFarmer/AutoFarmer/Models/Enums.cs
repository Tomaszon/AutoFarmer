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
		Left = 2,
		Right = 3,
		Upper = 5,
		Lower = 7,
		UpperLeft = 10,
		UpperRight = 15,
		LowerLeft = 14,
		LowerRight = 21
	}

	public enum AutoSearchAreaMode
	{
		LeftRight,
		UpperLower,
		Quarter
	}
}
