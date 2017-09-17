using PdxFile;
using System.Collections.Generic;
using System.Linq;

namespace Eu4Helper
{
	public class Eu4Area: Eu4ProvCollection
	{
		public float Prosperity { get; set; }

		public Eu4Region Region { get; set; }

		public Eu4Area(string name, PdxSublist value): base(name, value)
		{	
		}
	}
}