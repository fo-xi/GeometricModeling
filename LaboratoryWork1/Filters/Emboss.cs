using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaboratoryWork1.Filters
{
	public class Emboss : DotFilter
	{
		public const int STONE_OFFSET_X = 3;
		public const int STONE_OFFSET_Y = -3;

		public override unsafe bool TransformPix(ushort x, ushort y)
		{
			// Source
			byte* pSPix = GetPixelPointer(x, y, SOURCE);

			// Destination
			byte* pDPix = GetPixelPointer(x, y, DEST);

			if (pSPix == null || pDPix == null)
			{
				return false;
			}

			int x2 = x + STONE_OFFSET_X; if (x2 < 0) x2 = 0;
			int y2 = y + STONE_OFFSET_Y; if (y2 < 0) y2 = 0;

			// Offset pixel
			byte* pSPix2 = null;
			if ((pSPix2 = GetPixelPointer((ushort)x2, y2, SOURCE)) == null)
			{
				pSPix2 = pSPix;
			}

			// Brightness calculation	
			byte Y1, Y2;
			Y1 = Y(pSPix);
			Y2 = Y(pSPix2);

			// Finding the difference and moving it into the gray area
			byte d = (byte)((Y1 - Y2 + 255) / 2);

			// Pixel gets new values
			pDPix[0] = d;
			pDPix[1] = d;
			pDPix[2] = d;

			return true;
		}
	}
}
