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
}
