using PdxFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eu4Helper
{
	public abstract class Eu4ProvinceBase
	{
		public int ProvinceID { get; set; }

		public Eu4CountryBase Owner { get; set; }

		public List<Eu4CountryBase> Cores { get; set; }

		public List<float> Institutions { get; set; }

		public string Culture { get; set; }
		public string Religion { get; set; }

		public Dictionary<DateTime, string> CulturalHistory { get; set; }
		public Dictionary<DateTime, string> ReligiousHistory { get; set; }

		public int FortLevel { get; set; }

		public int BaseTax { get; set; }
		public int BaseProduction { get; set; }
		public int BaseManpower { get; set; }
		public int Development
		{
			get
			{
				return BaseTax + BaseProduction + BaseManpower;
			}
		}

		public string Estate { get; set; }

		public List<string> Flags { get; set; }

		public List<string> Buildings { get; set; }

		public Eu4Area Area { get; set; }

		public bool IsState { get; set; }
		public Eu4Continent Continent { get; set; }
		public string OriginalCulture { get; set; }

		
	}
}
