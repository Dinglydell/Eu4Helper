using PdxFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Eu4Helper
{

		public abstract class Eu4WorldBase
		{
			public static readonly string GAME_PATH = @"C:\Program Files (x86)\Steam\steamapps\common\Europa Universalis IV\";

			public string ModPath { get; set; }
			public string PlayerTag { get; set; }
			//public PdxSublist RootList { get; set; }

			public Dictionary<string, Eu4Religion> Religions { get; set; }
			public Dictionary<string, Eu4ReligionGroup> ReligiousGroups { get; set; }

			public List<string> Buildings { get; set; }
			public Dictionary<string, Eu4CountryBase> Countries { get; set; }
			public Dictionary<int, Eu4ProvinceBase> Provinces { get;  set; }
			public Dictionary<string, Eu4Culture> Cultures { get;  set; }
			public Dictionary<string, Eu4CultureGroup> CultureGroups { get;  set; }
			public Dictionary<string, string> Localisation { get;  set; }
			internal List<IEu4DiploRelation> Relations { get;  set; }
			public Dictionary<string, Eu4Area> Areas { get;  set; }
			public Dictionary<string, Eu4Region> Regions { get;  set; }
			public Dictionary<string, Eu4Continent> Continents { get; set; }

			public Dictionary<string, PdxSublist> CountryHistory { get; set; }
			public Dictionary<string, PdxSublist> ProvinceHistory { get; set; }
			public Eu4CountryBase GreatestPower { get; set; }

			public Eu4WorldBase(string modPath)
			{
			ModPath = modPath;
				Console.WriteLine("Loading vanilla EU4 data...");
			//	ModPath = modFilePath;
				LoadHistory();
				LoadRegions();
				LoadBuildingData();
				LoadLocalisation();
				

			Console.WriteLine("Vanilla EU4 data loaded.");
			}
		/// <summary>
		/// Loads things that need to be loaded after everything else is initialised. (eg religion and culture rely on the world being initialised)
		/// </summary>
		public void PostInitLoad()
		{
			LoadReligionData();
			LoadCultureData();
		}

			private void LoadHistory()
			{
				Console.WriteLine("Loading history..");
				CountryHistory = new Dictionary<string, PdxSublist>();
				var ctryHistory = GetFilesFor(@"history\countries");
				foreach (var ctry in ctryHistory)
				{
					CountryHistory.Add(Path.GetFileName(ctry).Substring(0, 3), PdxSublist.ReadFile(ctry));
				}

				//ProvinceHistory = new Dictionary<string, PdxSublist>();
				//var provHistory = GetFilesFor(@"history\provinces");
				//foreach (var prov in provHistory)
				//{
				//	ProvinceHistory.Add(Path.GetFileName(prov).Substring(0, 3), PdxSublist.ReadFile(prov));
				//}
			}

			private void LoadRegions()
			{
				Console.WriteLine("Loading EU4 areas..");
				var files = GetFilesFor("map");
				var areaFile = files.Find(f => Path.GetFileName(f) == "area.txt");
				var areas = PdxSublist.ReadFile(areaFile);
				Areas = new Dictionary<string, Eu4Area>();
				foreach (var ar in areas.Sublists)
				{
					//Areas[ar.Key] = new HashSet<int>(ar.Value.FloatValues.Values.SelectMany(f => f.Select(e => (int)e)));
					Areas[ar.Key] = new Eu4Area(ar.Key, ar.Value);
				}

				Console.WriteLine("Loading EU4 regions...");
				var regionFile = files.Find(f => Path.GetFileName(f) == "region.txt");
				var regions = PdxSublist.ReadFile(regionFile);
				Regions = new Dictionary<string, Eu4Region>();
				foreach (var reg in regions.Sublists)
				{
					Regions[reg.Key] = new Eu4Region(reg.Key, reg.Value, this);
				}

				Console.WriteLine("Loading EU4 areas..");

				var continentFile = files.Find(f => Path.GetFileName(f) == "continent.txt");
				var continents = PdxSublist.ReadFile(continentFile);
				Continents = new Dictionary<string, Eu4Continent>();
				foreach (var con in continents.Sublists)
				{
					//Areas[ar.Key] = new HashSet<int>(ar.Value.FloatValues.Values.SelectMany(f => f.Select(e => (int)e)));
					Continents[con.Key] = new Eu4Continent(con.Key, con.Value);
				}


			}

			

			private void LoadLocalisation()
			{
				Console.WriteLine("Loading EU4 localisation...");
				Localisation = new Dictionary<string, string>();
				var files = GetFilesFor("localisation");
				foreach (var file in files)
				{
					if (Path.GetFileNameWithoutExtension(file).EndsWith("l_english"))
					{
						LoadLocalisationFile(file);
					}
				}

			}

			private void LoadLocalisationFile(string path)
			{
				using (var file = new StreamReader(path))
				{

					var first = file.ReadLine().Trim();
					if (first == "l_english:")
					{

						var key = new StringBuilder();
						var value = new StringBuilder();
						var readKey = true;
						var readValue = false;
						var inQuotes = false;
						while (!file.EndOfStream)
						{
							var ch = Convert.ToChar(file.Read());
							if (char.IsWhiteSpace(ch) && !inQuotes)
							{
								if (!readKey)
								{
									if (readValue)
									{
										Localisation[key.ToString()] = value.ToString();
										key = new StringBuilder();
										value = new StringBuilder();
										readValue = false;
										readKey = true;
									}
									else
									{
										readValue = true;
									}
								}

								continue;
							}
							if (ch == '"')
							{
								inQuotes = !inQuotes;
								continue;
							}
							if (ch == ':' && readKey && !inQuotes)
							{
								readKey = false;
								continue;
							}
							if (readKey)
							{
								key.Append(ch);
							}
							if (readValue)
							{
								value.Append(ch);
							}
						}

						//Console.WriteLine(Localisation);
					}
				}
			}

			private void LoadBuildingData()
			{
				Buildings = new List<string>();
				var buildFiles = GetFilesFor(@"common\buildings");
				foreach (var buildFile in buildFiles)
				{
					var buildings = PdxSublist.ReadFile(buildFile);
					Buildings.AddRange(buildings.Sublists.Keys);
				}
			}

			private void LoadCultureData()
			{
				Cultures = new Dictionary<string, Eu4Culture>();
				CultureGroups = new Dictionary<string, Eu4CultureGroup>();
				var relFiles = GetFilesFor(@"common\cultures");
				var ignores = new string[] { "male_names", "female_names", "dynasty_names" };
				foreach (var relFile in relFiles)
				{
					var cultures = PdxSublist.ReadFile(relFile);
					cultures.ForEachSublist(culGroup =>
					{
						if (!CultureGroups.ContainsKey(culGroup.Key))
						{
							CultureGroups[culGroup.Key] = new Eu4CultureGroup(culGroup.Value, this);
						}
						culGroup.Value.ForEachSublist(cul =>
						{
							if (!Cultures.ContainsKey(cul.Key) && ignores.All(ign => ign != cul.Key))
							{
								Cultures[cul.Key] = CultureGroups[culGroup.Key].AddCulture(cul.Value, this);
							}
						});

					});
				}
			}

			private void LoadReligionData()
			{
				Religions = new Dictionary<string, Eu4Religion>();
				ReligiousGroups = new Dictionary<string, Eu4ReligionGroup>();
				var relFiles = GetFilesFor(@"common\religions");
				var rgx = new Regex(@"\d+$");
				foreach (var relFile in relFiles)
				{
					var religions = PdxSublist.ReadFile(relFile);
					religions.ForEachSublist(relGroup =>
					{
						var key = rgx.Replace(relGroup.Key, string.Empty);
						if (!ReligiousGroups.ContainsKey(key))
						{
							ReligiousGroups[relGroup.Key] = new Eu4ReligionGroup(key, this);
						}
						relGroup.Value.ForEachSublist(rel =>
						{
							if (!Religions.ContainsKey(rel.Key) && rel.Key != "flag_emblem_index_range")
							{
								Religions[rel.Key] = ReligiousGroups[key].AddReligion(rel.Value, this);
							}
						});

					});
				}

			}

			

			//private void LoadCountryTags()
			//{
			//	Console.WriteLine("Loading list of countries...");
			//	var countryTagFiles = GetFilesFor(@"common\country_tags");
			//	CountryTags = new List<string>();
			//	foreach (var ctf in countryTagFiles)
			//	{
			//		var file = new StreamReader(ctf);
			//		string line;
			//		while ((line = file.ReadLine()) != null)
			//		{
			//			if (line.First() !='#' && line.Contains('='))
			//			{
			//				var tag = line.Substring(0, line.IndexOf('=')).Trim();
			//				CountryTags.Add(tag);
			//			}
			//		}
			//	}
			//
			//	var dynCountries = RootList.Sublists["dynamic_countries"].Values.Select(s => s.Substring(1, 3));
			//	CountryTags.AddRange(dynCountries);
			//	Console.WriteLine(CountryTags[0]);
			//}

			public List<string> GetFilesFor(string path)
			{

				var modPath = Path.Combine(ModPath, path);
				var gameFiles = Directory.GetFiles(Path.Combine(GAME_PATH, path));
				var modFileNames = Directory.Exists(modPath) ? Directory.GetFiles(modPath).Select(Path.GetFileName) : new string[] { };
				var files = new List<string>();
				foreach (var name in gameFiles)
				{
					if (modFileNames.Contains(Path.GetFileName(name)))
					{
						files.Add(Path.Combine(modPath, Path.GetFileName(name)));
					}
					else
					{
						files.Add(name);
					}
				}
				foreach (var name in modFileNames)
				{
					var modFilePath = Path.Combine(modPath, Path.GetFileName(name));
					if (!files.Contains(modFilePath))
					{
						files.Add(modFilePath);
					}
				}
				return files;
			}

			
			public Eu4Religion GetReligion(string religion)
			{
				return Religions[religion];
			}

		
	}
}
