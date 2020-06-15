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
		ScanScreenForFadedUI
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
