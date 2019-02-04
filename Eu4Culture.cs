
using PdxFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eu4Helper
{
	public class Eu4Culture
	{
		public string Name { get; set; }
		public string DisplayName { get; set; }
		public Eu4CultureGroup Group { get; set; }
		public string PrimaryNation { get; set; }
		public List<string> MaleNames { get; set; }
		public List<string> FemaleNames { get; set; }
		public List<string> DynastyNames { get; set; }
		public Eu4Continent Continent { get; set; }
		public bool IsVanilla { get; set; }

		public Eu4Culture(string name, Eu4CultureGroup group, Eu4WorldBase world)
		{
			if (Name == "russian_culture")
			{
				Console.WriteLine();
			}
			IsVanilla = true;
			Name = name;
			Group = group;
			MaleNames = new List<string>();
			FemaleNames = new List<string>();
			DynastyNames = new List<string>();
		}

		public Eu4Culture(PdxSublist data, Eu4CultureGroup group, Eu4WorldBase world)
		{
			IsVanilla = true;
			Name = data.Key;
			DisplayName = world.Localisation.ContainsKey(Name) ? world.Localisation[Name] : Name;
			if (Name == "russian_culture")
			{
				Console.WriteLine();
			}
			if (Name == "english")
			{
				Console.WriteLine();
			}
			Group = group;
			var capital = 0;
			if (data.KeyValuePairs.ContainsKey("primary"))
			{
				PrimaryNation = data.GetString("primary");
				if (world.Countries.ContainsKey(PrimaryNation))
				{
					capital = world.Countries[PrimaryNation].Capital;
				}
			}
			if (capital == 0)
			{
				// highest development continent out of all eu4 provinces with that culture in 1444
				Continent = world.Provinces.Values.Where(p => p.OriginalCulture == Name).GroupBy(p => p.Continent).OrderByDescending(g => g.Sum(p => p.Development)).FirstOrDefault()?.Key;
			}
			else
			{
				Continent = world.Provinces[capital].Continent;
			}

			MaleNames = new List<string>();
			data.Sublists.ForEach("male_names", (sub) =>
			{
				MaleNames.AddRange(sub.Values);
			});// ? data.GetSublist("male_names").Values : new List<string>();
			FemaleNames = new List<string>();
			data.Sublists.ForEach("female_names", (sub) =>
			{
				FemaleNames.AddRange(sub.Values);
			});

			DynastyNames = data.Sublists.ContainsKey("dynasty_names") ? data.GetSublist("dynasty_names").Values : new List<string>();

		}

		public PdxSublist GetCultureData()
		{
			var data = new PdxSublist(null, Name);
			if (PrimaryNation != null)
			{
				data.AddValue("primary", PrimaryNation);
			}

			
			
			data.AddSublist("male_names", PdxSublist.FromList(MaleNames));
			

			data.AddSublist("female_names", PdxSublist.FromList(FemaleNames));
			if (DynastyNames.Count > 0)
			{
				data.AddSublist("dynasty_names", PdxSublist.FromList(DynastyNames));
			}



			return data;
		}
		public void AddLocalisation(Dictionary<string, string> localisation)
		{
			if (DisplayName != null && !IsVanilla)
			{
				localisation.Add(Name, DisplayName);
			}
		}
	}

	public class Eu4CultureGroup
	{
		public string Name { get; set; }
		public List<Eu4Culture> Cultures { get; set; }
		public string DisplayName { get; internal set; }
		public string GraphicalCulture { get; private set; }
		public bool AnyNew { get { return Cultures.Any(c => !c.IsVanilla); } }

		public Eu4CultureGroup(PdxSublist data, Eu4WorldBase world) : this(data.Key)
		{

			GraphicalCulture = data.KeyValuePairs.ContainsKey("graphical_culture") ? data.GetString("graphical_culture") : null;
			DisplayName = world.Localisation[data.Key];


			//foreach(var sub in data.Sublists)
			//{
			//	if (sub.Value.KeyValuePairs.ContainsKey("primary"))
			//	{
			//		Cultures.Add(new Eu4Culture(sub.Value, this));
			//	}
			//}
		}
		public Eu4CultureGroup(string name)
		{
			Name = name;
			Cultures = new List<Eu4Culture>();
		}

		public Eu4Culture AddCulture(PdxSublist data, Eu4WorldBase world)
		{
			var culture = new Eu4Culture(data, this, world);
			Cultures.Add(culture);
			return culture;
		}
		public Eu4Culture AddCulture(string name, Eu4WorldBase world, bool vanilla = true, string display = null)
		{
			var culture = new Eu4Culture(name, this, world);
			culture.DisplayName = display;
			Cultures.Add(culture);
			if (!vanilla)
			{
				culture.IsVanilla = false;
			}
			return culture;
		}

		public PdxSublist GetGroupData()
		{
			var data = new PdxSublist(null, Name);
			data.AddValue("graphical_culture", GraphicalCulture);
			Cultures.ForEach((culture) =>
			{
				data.AddSublist(culture.Name, culture.GetCultureData());
			});
			return data;
		}
		public void AddLocalisation(Dictionary<string, string> localisation)
		{
			if (DisplayName != null)
			{
				localisation.Add(Name, DisplayName);
			}
			foreach(var cul in Cultures)
			{
				cul.AddLocalisation(localisation);
			}
		}
	}
}
