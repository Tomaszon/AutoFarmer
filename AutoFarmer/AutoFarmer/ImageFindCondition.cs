namespace AutoFarmer
{
	public class ImageFindCondition
	{
		public string TemplateName { get; set; }

		public string SearchRectangleName { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is null) return false;

			if (obj is ImageFindCondition c)
			{
				return TemplateName == c.TemplateName && SearchRectangleName == c.SearchRectangleName;
			}

			return false;
		}
	}
}
