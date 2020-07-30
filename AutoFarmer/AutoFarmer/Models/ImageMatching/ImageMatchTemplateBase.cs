namespace AutoFarmer.Models.ImageMatching
{
	public abstract class ImageMatchTemplateBase
	{
		public string Name { get; set; }

		public ImageMatchTemplateBase(string name)
		{
			Name = name;
		}
	}
}
