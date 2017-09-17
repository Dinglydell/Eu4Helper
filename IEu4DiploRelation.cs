using PdxFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eu4Helper
{
	public enum Relation {
		alliance, dependency, military_access
	}

	public interface IEu4DiploRelation
	{
		Relation Type { get; set; }
		Eu4CountryBase First { get; set; }
		Eu4CountryBase Second { get; set; }

		string SubjectType { get; set; }
	
	}
}
