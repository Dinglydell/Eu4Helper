using PdxFile;
using System.Collections.Generic;
using System.Linq;

namespace Eu4Helper
{
	public class Eu4SuperRegion
	{
		public string Name { get; set; }

		public HashSet<Eu4Region> Regions { get; set; }

		public Eu4SuperRegion(string name, PdxSublist value, Eu4WorldBase world)
		{
			Name = name;

			Regions = new HashSet<Eu4Region>(value.Values.Where(an => world.Regions.ContainsKey(an)).Select(an => world.Regions[an]));
			foreach (var region in Regions)
			{
				if (region.SuperRegion != null)
				{
					System.Console.WriteLine($"WARNING: {region.Name} exists in multiple regions!");
				}
				region.SuperRegion = this;
			}
		}
	}
}
