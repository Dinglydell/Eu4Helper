using PdxFile;
using PdxUtil;
using System;
using System.Collections.Generic;

namespace Eu4Helper
{
	public class Eu4Religion
	{
		public Eu4ReligionGroup Group { get; set; }
		public Colour Colour { get; set; }
		public int Icon { get; set; }
		public string DisplayName { get; internal set; }
		public string Name { get; set; }

		public Eu4Religion(PdxSublist data, Eu4ReligionGroup group, Eu4WorldBase world) : this(data.Key, group, world)
		{
			
			Colour = new Colour(data.GetSublist("color").FloatValues[string.Empty]);
			Icon = (int)data.GetFloat("icon");
			DisplayName = world.Localisation[data.Key];
			
		}

		public Eu4Religion(string name, Eu4ReligionGroup group, Eu4WorldBase world)
		{
			Name = name;
			Group = group;
		}
	}

	public class Eu4ReligionGroup
	{
		public string Name { get; private set; }
		public List<Eu4Religion> Religions { get; set; }
		public string DisplayName { get; internal set; }

		public Eu4ReligionGroup(string name, Eu4WorldBase world): this(name, world, world.Localisation[name])
		{

		}

		public Eu4ReligionGroup(string name, Eu4WorldBase world, string displayName)
		{
			Name = name;
			Religions = new List<Eu4Religion>();
			DisplayName = displayName;
		}

		public Eu4Religion AddReligion(PdxSublist value, Eu4WorldBase world)
		{
			var religion = new Eu4Religion(value, this, world);
			Religions.Add(religion);
			return religion;
		}

		public Eu4Religion AddReligion(string name, Eu4WorldBase world)
		{
			var religion = new Eu4Religion(name, this, world);
			Religions.Add(religion);
			return religion;
		}
	}
}