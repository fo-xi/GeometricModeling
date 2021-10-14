using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaboratoryWork1
{
    public class MatrixFilter: Filter
    {
        protected int rangX; // X and Y size of matrix
        protected int rangY;
        protected int[,] matrix; // matrix pointer

        // Pixel transformation method
        public override unsafe bool TransformPix(ushort x, ushort y)
        {
            int x_start = 0;
            int dx = rangX / 2, dy = rangY / 2;
            if (x - dx < 0)
                x_start = dx - x;

            int y_start = 0;
            if (y - dy < 0)
                y_start = dy - y;

            int x_finish = rangX;

            if (x + dx > Buffer[SOURCE].Width)
                x_finish -= (x + dx - Buffer[SOURCE].Width);

            int y_finish = rangY;
            if (y + dy > Buffer[SOURCE].Height)
                y_finish -= (y + dy - Buffer[SOURCE].Height);

            // Calculating new pixel color values taking into
            // account neighbors falling into the transformation
            // matrix coverage area.
            int[] NewBGR = new int[3];
            int count = 0;
            for (int c = 0, mx = 0, my = 0; c < 3; c++)
            {
                NewBGR[c] = 0; count = 0;
                for (my = y_start; my < y_finish; my++)
                    for (mx = x_start; mx < x_finish; mx++)
                    {
                        // Source 
                        byte* pSPix = null;
                        if ((pSPix = GetPixelPointer((ushort)(x + (mx - dx)), (ushort)(y + (my - dy)), SOURCE)) != null)
                        {
                            NewBGR[c] += (matrix[my, mx] * pSPix[c]);
                            count += matrix[my, mx];
                        }

                    }
            }

            // Pixel address in the destination image
            byte* pDPix = GetPixelPointer(x, y, DEST);

            // Reducing the value to the allowed range and setting into the destination
            if (pDPix != null)
                for (int c = 0; c < 3; c++)
                {
                    if (count != 0)
                        NewBGR[c] = NewBGR[c] / count;
                    if (NewBGR[c] < 0)
                        NewBGR[c] = 0;
                    else if (NewBGR[c] > 255)
                        NewBGR[c] = 255;

                    pDPix[c] = (byte)NewBGR[c];
                }

            return true;
        }
    }
}
