using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace LaboratoryWork1
{
    public abstract class Filter
    {
        public readonly static int SOURCE = 0;

        public readonly static int DEST = 1;

        private Bitmap[] BM;

        // Bitmap data
        protected BitmapData[] Buffer;

        public Filter()
        {
            BM = new Bitmap[2];
            Buffer = new BitmapData[2];
        }

        public static unsafe byte Y(byte* pPix)
        {
            return ((byte)(0.11 * pPix[0] + 0.59 * pPix[1] + 0.3 * pPix[2]));
        }

        public bool SetBuffers(Bitmap source, Bitmap dest)
        {
            BM[SOURCE] = source;
            BM[DEST] = dest;

            if (BM[SOURCE] == null || BM[DEST] == null)
            {
                return false;
            }

            // return result
            bool result = false;

            try
            {
                // Getting direct access to data            
                if (// indicating that source read only
                    (Buffer[SOURCE] = BM[SOURCE].LockBits(new Rectangle(Point.Empty, BM[SOURCE].Size),
                    ImageLockMode.ReadOnly, BM[SOURCE].PixelFormat)) == null ||
                    // indicating that destination write mode
                    (Buffer[DEST] = BM[DEST].LockBits(new Rectangle(Point.Empty, BM[DEST].Size),
                    ImageLockMode.WriteOnly, BM[DEST].PixelFormat)) == null)
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public bool ReleaseBuffers()
        {
            // Check
            if (BM[SOURCE] == null || BM[DEST] == null)
            {
                return false;
            }

            // Releasing buffers
            BM[SOURCE].UnlockBits(Buffer[SOURCE]);
            BM[DEST].UnlockBits(Buffer[DEST]);

            return true;
        }

        public unsafe byte* GetPixelPointer(ushort x, int y, int source)
        {
            if (source != 1 && source != 0)
            {
                return null;
            }

            if (Buffer[source].Scan0 == IntPtr.Zero || x >= Buffer[source].Width || y >= Buffer[source].Height)
            {
                return null;
            }

            // Byte per pixel calculation
            byte bpp = 0;

            unchecked
            {
                bpp = (byte)(((byte)((int)Buffer[source].PixelFormat >> 8)) / 8);
            }

            // Address determination
            return ((byte*)Buffer[source].Scan0) + Buffer[source].Stride * y + x * bpp;
        }

        public unsafe bool CopyPix(ushort x1, ushort x2, ushort y)
        {
            byte* pSPix1 = GetPixelPointer(x1, y, SOURCE);
            byte* pSPix2 = GetPixelPointer(x2, y, SOURCE);

            // Destination 
            byte* pDPix = GetPixelPointer(x1, y, DEST);

            if (pSPix1 == null || pSPix2 == null || pDPix == null)
            {
                return false;
            }

            int lenght = (int)(pSPix2 - pSPix1);

            byte[] tempArray = new byte[lenght];
            Marshal.Copy(new IntPtr(pSPix1), tempArray, 0, lenght);
            Marshal.Copy(tempArray, 0, new IntPtr(pDPix), lenght);

            return true;
        }

        public abstract bool TransformPix(ushort x, ushort y);
    }
}
