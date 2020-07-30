using AutoFarmer.Models.Graph;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AutoFarmer.Models.Graph.IOptions;

namespace AutoFarmer.Models.ImageMatching
{
	public class ImageMatchTemplateOptions : ImageMatchTemplateBase, IOptions
	{
		public Dictionary<string, SearchRectangleOptions>? SearchRectangles { get; set; }

		public ImageMatchTemplateOptions(string name) : base(name) { }

		public static ImageMatchTemplateOptions FromJsonFile(string path)
		{
			return FromJsonFileWrapper(() =>
			{
				var templateOptions = new ImageMatchTemplateOptions(Path.GetFileNameWithoutExtension(path));

				JsonConvert.PopulateObject(File.ReadAllText(path), templateOptions);

				return templateOptions;
			});
		}

		public ImageMatchTemplate ToImageMatchTemplate()
		{
			return new ImageMatchTemplate(Name, SearchRectangles.ToDictionary(t => t.Key, v => v.Value.ToSearchRectangle()));
		}
	}
}
