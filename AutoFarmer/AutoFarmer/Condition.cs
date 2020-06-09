namespace AutoFarmer
{
	public class Condition
	{
		public string TemplateName { get; set; }

		public string SearchRectangleName { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is null) return false;

			if (obj is Condition c)
			{
				return TemplateName == c.TemplateName && SearchRectangleName == c.SearchRectangleName;
			}

			return false;
		}
	}
}
