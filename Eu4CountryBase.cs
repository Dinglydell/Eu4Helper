using Eu4Helper;
using PdxFile;
using PdxUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eu4Helper
{
	

	public class Estate
	{
		public static readonly string[] EstateTypes = new string[] { null, "estate_church", "estate_nobles", "estate_burghers", "estate_cossacks", "estate_nomadic_tribes", "estate_dhimmi"};
		public string Type { get; set; }
		public float Loyalty { get; set; }
		public float Influence { get; set; }
		public float Territory { get; set; }

		public Estate(PdxSublist estate)
		{
			Type = estate.GetString("type");
			Loyalty = estate.GetFloat("loyalty");
			Influence = estate.GetFloat("influence");
			Territory = estate.FloatValues.ContainsKey("territory") ? estate.GetFloat("territory") : 0;
		}
	}

	public abstract class Eu4CountryBase
	{
		//Todo: better system here
		public static readonly string[] INSTITUTION_NAMES = new string[] { "feudalism", "renaissance", "new_world_i", "printing_press", "global_trade", "manufactories", "enlightenment" };
		public bool Exists { get; set; }

		public Eu4WorldBase World { get; set; }

		public string DisplayNoun { get; set; }
		public string DisplayAdj { get; set; }

		public byte GovernmentRank { get; set; }

		public Dictionary<string, bool> Institutions { get; set; }
		public string CountryTag { get; set; }
		public string Overlord { get; set; }
		public List<string> Subjects { get; set; }
		public float LibertyDesire { get; set; }

		public int Capital { get; set; }

		public Colour MapColour { get; set; }

		public string PrimaryCulture { get; set; }
		public List<string> AcceptedCultures { get; set; }
		public string Religion { get; set; }

		

		public byte AdmTech { get; set; }
		public byte DipTech { get; set; }
		public byte MilTech { get; set; }

		public List<Estate> Estates { get; set; }

		public float PowerProjection { get; set; }

		public DateTime LastElection { get; set; }

		public float Prestige { get; set; }
		public sbyte Stability { get; set; }
		public float Inflation { get; set; }


		public int Debt { get; set; }

		public float Absolutism { get; set; }
		public float Legitimacy { get; set; }
		public float RepublicanTradition { get; set; }
		public float Corruption { get; set; }
		public float Mercantilism { get; set; }

		public int Development { get
			{
				return World.Provinces.Where(p => p.Value.Owner == this).Sum(p => p.Value.Development);
			}
		}

		public int GreatPowerScore
		{
			get {
				return (Development + Subjects.Sum(s => World.Countries[s].Development) / 2) / (int)(1 + 0.5 * Institutions.Count(i => !i.Value));
			}
		}

		public Dictionary<string, byte> Ideas { get; set; }

		public string Government { get; set; }

		public List<string> Flags { get; set; }

		public List<string> Policies { get; set; }
		public bool IsColonialNation { get; set; }
		public List<int> Opinions { get; set; }
		public HashSet<Eu4Area> States { get; set; }
		public bool IsVanilla { get; set; }
		public bool InHRE { get; set; }
		public bool IsElector { get; set; }

		public Eu4CountryBase()
		{
			IsVanilla = true;
		}
		public Eu4CountryBase(Eu4WorldBase world)
		{
			World = world;
		}


		public abstract PdxSublist GetHistoryFile();
		public abstract PdxSublist GetCountryFile();
		public abstract void AddDiplomacy(PdxSublist diplomacy);
	}
}
