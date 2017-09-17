using Eu4Helper;
using PdxFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eu4Helper
{
	public class Colour
	{
		public byte Red { get; set; }
		public byte Green { get; set; }
		public byte Blue { get; set; }

		public Colour(byte r, byte g, byte b)
		{
			Red = r;
			Green = g;
			Blue = b;
		}


		public Colour(List<float> rgb) 
		{
			
			byte r;
			byte g;
			byte b;
			if(byte.TryParse(rgb[0].ToString(), out r) && byte.TryParse(rgb[1].ToString(), out g) && byte.TryParse(rgb[2].ToString(), out b))
			{
				Red = r;
				Green = g;
				Blue = b;
			} else
			{
				Red = (byte)(rgb[0] * 255);
				Green = (byte)(rgb[1] * 255);
				Blue = (byte)(rgb[2] * 255);
			}
		}

		public Colour(List<float> rgb, byte multiplier): this((byte)(multiplier * rgb[0]), (byte)(multiplier * rgb[1]), (byte)(multiplier * rgb[2]))
		{

		}

	}

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
			Territory = estate.GetFloat("territory");
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

		
	}
}
