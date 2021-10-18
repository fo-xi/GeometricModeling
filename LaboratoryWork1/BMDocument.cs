using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace LaboratoryWork1
{
    public class BMDocument
    {
		// public events

		// update all views
		public event EventHandler DocumentInvalid;

		// views array
		private ArrayList viewList = new ArrayList();

		// current fileName
		private string fileName;

		// percent for transaction
		private int executedPercent = 0;

		// Event. When setted - transaction ends anyway
		public ManualResetEvent EventDoTransform = new ManualResetEvent(false);

		// data section
		private Bitmap[] bufferBM = new Bitmap[2];    // Two images (one for undo)
		private Bitmap curBM = null;                // Current image

		private Filter currentFilter = null;
		private bool editHalf = false;
		private bool modified = false;
		private bool editable = true;

		// for printing
		private int pageCounter = 0;

		public BMDocument(string fileName)
		{
			this.fileName = fileName;
			bufferBM[0] = (Bitmap)Bitmap.FromFile(fileName);
			curBM = bufferBM[0];
		}

		/// <summary>
		/// Get view count
		/// </summary>
		public int ViewCount
		{
			get
			{
				return this.viewList.Count;
			}
		}

		/// <summary>
		/// Get/Set modofied flag
		/// </summary>
		public bool Modified
		{
            get
            {
                return this.modified;
            }
            set
            {
                this.modified = value;
            }
		}

		/// <summary>
		/// Get if the undo operation is enabled now
		/// </summary>
		public bool UndoEnabled
		{
			get
			{
				return this.GetBufferBMPtr() != null;
			}
		}

		/// <summary>
		/// Can be filter apply to document now?
		/// </summary>
		public bool Editable
		{
			get
			{
				return this.editable;
			}
		}

		/// <summary>
		/// Get executed percent [0..100]
		/// </summary>
		public int ExecutedPercent
		{
			get { return executedPercent; }
		}

		/// <summary>
		/// Get filename of the document
		/// </summary>
		public string FileName
		{
            get
            {
                return fileName;
            }
			set
			{
				this.fileName = value;
				// invalidate all view to show changes
				this.OnDocumentInvalid();
			}
		}

		/// <summary>
		/// Get current Bitmap
		/// </summary>
		public Bitmap CurrentBM
		{
			get
			{
				return this.curBM;
			}
		}

		/// <summary>
		/// Get current bitmap size
		/// </summary>
		public Size CurrentBMSize
		{
			get
			{
				return this.curBM.Size;
			}
		}

		/// <summary>
		/// Get/Set Edit half property. When set to true
		/// filters will modify only half of the image
		/// </summary>
		public bool EditHalf
		{
			set
			{
				this.editHalf = value;
			}
			get
			{
				return this.editHalf;
			}
		}

		#region GET/SET/CREATE Buffer

		private int GetNCurrentBM()
		{
            if (curBM == null || curBM == bufferBM[0])
            {
                return 0;
            }
            else
            {
                return 1;
            }
		}

		private bool CreateCompatibleBuffer()
		{
            if (curBM == null)
            {
                return false;
            }

			Bitmap pBuff = GetBufferBMPtr();

			if (pBuff != null)// && width && height && (width!=pBuff->GetWidth() || height!=pBuff->GetHeight()))
			{
				pBuff.Dispose();
				pBuff = null;
			}

			if (pBuff == null)
			{
				PixelFormat format = curBM.PixelFormat;
				pBuff = new Bitmap(curBM.Width, curBM.Height, format);
			}

			SetBufferBMPtr(pBuff);
			return pBuff != null;
		}

		private Bitmap GetCurrentBMPtr()
		{
			return curBM;
		}

		private Bitmap GetBufferBMPtr()
		{
			return bufferBM[1 - GetNCurrentBM()];
		}

		private void SetBufferBMPtr(Bitmap bmp)
		{
			bufferBM[1 - GetNCurrentBM()] = bmp;
		}

		private void SwapBM()
		{
			curBM = GetBufferBMPtr();
		}


		#endregion

		/// <summary>
		/// Method, which start Transform thread
		/// </summary>
		private async Task Transform()
		{
			var task = new Task(TransformLoop);
			task.Start();
			await task;
		}

		/// <summary>
		/// Main transform loop
		/// </summary>
		private void TransformLoop()
		{
            if (this.currentFilter == null)
            {
                return;
            }

            if (!CreateCompatibleBuffer())
            {
                return;
            }

			//reseting event
			EventDoTransform.Reset();
			this.editable = false;

            Bitmap pSBM = GetCurrentBMPtr();    // source
			Bitmap pDBM = GetBufferBMPtr();     // destinaiton

			if (!currentFilter.SetBuffers(pSBM, pDBM))
			{
				EventDoTransform.Reset();
				this.editable = true;

				MessageBox.Show("Cannot access data");
				return;
			}

			ushort width = (ushort)pSBM.Width;
			ushort height = (ushort)pSBM.Height;

			for (ushort y = 0; y < height; y++)
			{
				// Completion percentage
				Interlocked.Exchange(ref executedPercent, 100 * y / height);
				// Check if the user decided to interrupt execution
				if (EventDoTransform.WaitOne(0, true))
				{
					currentFilter.ReleaseBuffers();
					this.editable = true;
                    return;
				}

				for (ushort x = 0; x < width; x++)
				{
					if (editHalf && x == 0)
					{
						currentFilter.CopyPix(x, (ushort)(width / 2), y);
						x = (ushort)(width / 2 - 1);
					}
                    else
                    {
                        this.currentFilter.TransformPix(x, y);
                    }
				}
			}

			EventDoTransform.Reset();
			this.editable = true;

			if (!currentFilter.ReleaseBuffers())
			{
				MessageBox.Show("Can't release buffers");
				return;
			}

			SwapBM();           // make the buffer current image
			this.modified = true; // set "data changed" flag                
        }

		// call OnDocument invalid event
		protected void OnDocumentInvalid()
		{
			if (this.DocumentInvalid != null)
			{
				this.DocumentInvalid(this, EventArgs.Empty);
			}
		}

		public async Task BrightnessAndContrast(int ContrastOffset)
		{
			currentFilter = new BrightnessAndContrast();
			((BrightnessAndContrast)currentFilter).Init(0, ContrastOffset);
			await Transform();
		}

		public async Task InvertColors()
		{
			currentFilter = new InvertColors();
			await Transform(); ;
		}

		public async Task Sharp()
		{
			currentFilter = new Sharp();
			await Transform();
		}

		internal void SaveFile()
		{
			try
			{
				this.CurrentBM.Save(this.fileName);
				this.Modified = false;
			}
			catch (Exception ex)
			{
				MessageBox.Show(String.Format("Couldn't save file :  {0}", ex.Message), "BWViewer");
			}
		}

		public void SubscribeToPrint(PrintDocument doc)
		{
			doc.BeginPrint += new PrintEventHandler(doc_BeginPrint);
			doc.PrintPage += new PrintPageEventHandler(doc_PrintPage);
		}

		private void doc_BeginPrint(object sender, PrintEventArgs e)
		{
			this.pageCounter = 0;
		}

		private void doc_PrintPage(object sender, PrintPageEventArgs e)
		{
			// ptinting current BM
			Size drSize = new Size(
				e.PageSettings.PaperSize.Width -
				e.PageSettings.Margins.Left - e.PageSettings.Margins.Right,

				e.PageSettings.PaperSize.Height -
				e.PageSettings.Margins.Top - e.PageSettings.Margins.Bottom);

			if (e.PageSettings.Landscape)
			{
				drSize = new Size(drSize.Height, drSize.Width);
			}

			int howmanyX = this.CurrentBMSize.Width / drSize.Width + 1;
			int howmanyY = this.CurrentBMSize.Height / drSize.Height + 1;

			int pages = howmanyX * howmanyY;

			if (this.pageCounter == (pages - 1))
			{
				e.HasMorePages = false;
			}
            else
            {
                e.HasMorePages = true;
            }

			int y = this.pageCounter / howmanyX;
			int x = this.pageCounter - y * howmanyX;

			Rectangle rect = new Rectangle(x * e.MarginBounds.Width, y * e.MarginBounds.Height, e.MarginBounds.Width, e.MarginBounds.Height);

			e.Graphics.DrawImage(this.curBM, e.MarginBounds, rect, GraphicsUnit.Pixel);
			this.pageCounter++;
		}

		public void UnsubscribeToPrint(PrintDocument doc)
		{
			doc.BeginPrint -= new PrintEventHandler(doc_BeginPrint);
			doc.PrintPage -= new PrintPageEventHandler(doc_PrintPage);
		}

        public static Bitmap BitmapSourceToBitmap(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }

        public BMDocument(ImageSource imageSource)
        {
	        bufferBM[0] = BitmapSourceToBitmap(imageSource as BitmapSource);
			curBM = bufferBM[0];
        }

        public ImageSource ConvertImageSourceToBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException("bitmap");
            }

            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);

            try
            {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
		}
    }
}
