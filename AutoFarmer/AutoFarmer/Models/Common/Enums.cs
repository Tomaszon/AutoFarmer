namespace AutoFarmer.Models.Common
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
		LeftHold,
		LeftDoubleClick,
		ScanScreenForFadedUI,
		Move
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
		Top = 0b1110,
		Bottom = 0b1010,
		TopLeft = 0b1100,
		TopRight = 0b0100,
		BottomLeft = 0b1000,
		BottomRight = 0b0000,
		MiddleHalfWidth,
		MiddleHalfHeight,
		Middle
	}

	public enum ResultAppendMode
	{
		Override,
		Append,
		None
	}

	public enum ConditionMode
	{
		Primitive,
		Or,
		And
	}
}
