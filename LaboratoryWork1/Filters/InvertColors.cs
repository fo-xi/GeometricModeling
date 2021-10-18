using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaboratoryWork1
{
	public class InvertColors : DotFilter
	{
		public InvertColors()
		{
			for (int i = 0, t = 0; t < 3; t++)
			for (i = 0; i < 256; i++)
				this.BGRTransTable[t, i] = (byte)(255 - i);
		}
	}
}
