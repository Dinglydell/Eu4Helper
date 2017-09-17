﻿using PdxFile;
using System.Collections.Generic;
using System.Linq;

namespace Eu4Helper
{
	public class Eu4Region
	{
		public string Name { get; set; }

		public HashSet<Eu4Area> Areas { get; set; }

		public Eu4Region(string name, PdxSublist value, Eu4WorldBase world)
		{
			Name = name;
			
			if (value.Sublists.ContainsKey("areas"))
			{
				Areas = new HashSet<Eu4Area>(value.Sublists["areas"].Values.Select(an => world.Areas[an]));
				foreach (var area in Areas)
				{
					if(area.Region != null)
					{
						System.Console.WriteLine($"WARNING: {area.Name} exists in multiple regions!");
					}
					area.Region = this;
				}
			}
		}
	}
}