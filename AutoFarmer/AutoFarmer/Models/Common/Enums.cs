﻿namespace AutoFarmer.Models.Common
{
	public enum NotificationType
	{
		None,
		Click,
		ClickSingle,
		Error,
		Info
	}

	public enum MouseAction
	{
		LeftClick,
		LeftDoubleClick,
	}

	public enum SpecialAction
	{
		LeftHold,
		ScanScreenForFadedUI,
		Move,
		Multiply
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

	/// <summary>
	/// Search areas. Format: xx_yy_www_hhh in quarters
	/// </summary>
	public enum NamedSearchArea
	{
		Full = 0b__00_00_100_100,
		Left = 0b__00_00_010_100,
		Right = 0b__10_00_010_100,
		Top = 0b__00_00_100_010,
		Bottom = 0b__00_10_100_010,
		TopLeft = 0b__00_00_010_010,
		TopRight = 0b__10_00_010_010,
		BottomLeft = 0b__00_10_010_010,
		BottomRight = 0b__10_10_010_010,
		MiddleHalfWidth = 0b__01_00_010_100,
		MiddleHalfHeight = 0b__00_01_100_010,
		Middle = 0b__01_01_010_010
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

	public enum ReportMessageType
	{
		Success,
		Fail
	}
}
