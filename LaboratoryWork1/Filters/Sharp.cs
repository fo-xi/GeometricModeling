using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaboratoryWork1
{
	public class Sharp : MatrixFilter
	{
		public const int SHARP_COEFF = 3;

		public Sharp()
		{
			this.rangX = 5;
			this.rangY = 5;
			this.matrix = new int[5, 5]{
				{1,1,1,1,1},
				{1,1,1,1,1},
				{1,1,1,1,1},
				{1,1,1,1,1},
				{1,1,1,1,1}};
		}

		public override unsafe bool TransformPix(ushort x, ushort y)
		{
			// Dithering the pixel
            if (!base.TransformPix(x, y))
            {
                return false;
            }

			// Source
			byte* pSPix = GetPixelPointer(x, y, SOURCE);

			// Destination
			byte* pDPix = GetPixelPointer(x, y, DEST);

            if (pSPix == null || pDPix == null)
            {
                return false;
            }

			int d = 0;

			for (int c = 0; c < 3; c++)
			{
				// finding difference
				d = pSPix[c] - pDPix[c];
				// Amplifying the difference
				d *= SHARP_COEFF;
				// Assigning new value to pixel
                if ((int)pDPix[c] + d < 0)
                {
                    pDPix[c] = 0;
                }
				else if (pDPix[c] + d > 255)
                {
                    pDPix[c] = 255;
                }
                else
                {
                    pDPix[c] += (byte)d;
                }
			}

			return true;
		}
	}
}
