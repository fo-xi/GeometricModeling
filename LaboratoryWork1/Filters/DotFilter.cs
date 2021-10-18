using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaboratoryWork1
{
    public class DotFilter: Filter
    {
		protected byte[,] BGRTransTable = new byte[3, 256];

		public override unsafe bool TransformPix(ushort x, ushort y)
		{
			// Source
			byte* pSPix = GetPixelPointer(x, y, SOURCE);
			// Destination
			byte* pDPix = GetPixelPointer(x, y, DEST);

			if (pSPix == null || pDPix == null)
				return false;

			// Transforming and setting pixel in the source image
			pDPix[0] = BGRTransTable[0, pSPix[0]];
			pDPix[1] = BGRTransTable[1, pSPix[1]];
			pDPix[2] = BGRTransTable[2, pSPix[2]];

			return true;
		}
	}
}
