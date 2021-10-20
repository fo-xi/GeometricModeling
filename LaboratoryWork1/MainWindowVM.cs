using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Windows;
using System.Windows.Documents.DocumentStructures;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System.Windows.Media;
using LaboratoryWork1.Service;
using Image = System.Windows.Controls.Image;

namespace LaboratoryWork1
{
	public class MainWindowVM : ViewModelBase
	{
		private string _imagePath;

		private double _scale;

		public RelayCommand<Image> AddImageCommand { get; set; }

		public RelayCommand<Image> RightRotationCommand { get; set; }

		public RelayCommand<Image> LeftRotationCommand { get; set; }

		public RelayCommand<Image> HorizontalReflectionCommand { get; set; }

		public RelayCommand<Image> VerticalReflectionCommand { get; set; }

		public RelayCommand<Image> ScalingCommand { get; set; }

		public RelayCommand<Image> SaveCommand { get; set; }

		public RelayCommand<Image> SaveAsCommand { get; set; }

        public RelayCommand<Image> BrightnessAndContrastCommand { get; set; }

        public RelayCommand<Image> InvertColorsCommand { get; set; }

        public RelayCommand<Image> SharpCommand { get; set; }

		public RelayCommand<Image> BlurCommand { get; set; }

		public RelayCommand<Image> EmbossCommand { get; set; }

		public RelayCommand<Image> ContourCommand { get; set; }

		private IBrightnessAndContrastWindowService _brightnessAndContrasWindowService;

		public string ImagePath
		{
			get
			{
				return _imagePath;
			}
			set
			{
				_imagePath = value;
				RaisePropertyChanged(nameof(ImagePath));
			}
		}

		public string Scale
		{
			get
			{
				return _scale.ToString();
			}
			set
			{
				_scale = double.Parse(value);
				RaisePropertyChanged(nameof(Scale));
			}
		}

		private void AddImage(Image image)
		{
			var fileDialog = new OpenFileDialog
			{
				Title = "Open Image",
				Filter = "*.png|*.png|*.jpeg|*.jpeg|*.bmp|*.bmp"
			};
			if ((bool)fileDialog.ShowDialog())
			{
				ImagePath = fileDialog.FileName;
				using (var fileStream = new FileStream(ImagePath, FileMode.Open))
				{
					image.Source = BitmapFrame.Create(fileStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
				}
			}
		}

		public void SaveAsFileDialog(Image image)
		{
			var fileDialog = new SaveFileDialog()
			{
				Title = "Save Image",
				Filter = "*.png|*.png|*.jpeg|*.jpeg|*.bmp|*.bmp"
			};

			if ((bool)fileDialog.ShowDialog())
			{
				BitmapEncoder encoder = GetEncoder(Path.GetExtension(fileDialog.FileName));
				var bitmap = image.Source as BitmapSource;
				encoder.Frames.Add(BitmapFrame.Create(bitmap));

				using (var fileStream = new FileStream(fileDialog.FileName, FileMode.OpenOrCreate))
				{
					encoder.Save(fileStream);
				}
			}
		}

		public void SaveFileDialog(Image image)
		{
			BitmapEncoder encoder = GetEncoder(Path.GetExtension(ImagePath));
			var bitmap = image.Source as BitmapSource;
			encoder.Frames.Add(BitmapFrame.Create(bitmap));

			using (var fileStream = new FileStream(ImagePath, FileMode.OpenOrCreate))
			{
				encoder.Save(fileStream);
			}
		}

		private void Rotation(Image image, double angle)
		{
			var bitmap = image.Source as BitmapSource;
			RotateTransform transform = new RotateTransform(angle);
			TransformedBitmap transformedBitmap = new TransformedBitmap(bitmap, transform);
			image.Source = transformedBitmap;
		}

		private void RightRotation(Image image)
		{
			Rotation(image, 90);
		}

		private void LeftRotation(Image image)
		{
			Rotation(image, -90);
		}

		private void Reflection(Image image, double axisX, double axisY)
		{
			var bitmap = image.Source as BitmapSource;
			ScaleTransform transform = new ScaleTransform(axisX, axisY);
			TransformedBitmap transformedBitmap = new TransformedBitmap(bitmap, transform);
			image.Source = transformedBitmap;
		}

		private void HorizontalReflection(Image image)
		{
			Reflection(image, -1, 1);
		}

		private void VerticalReflection(Image image)
		{
			Reflection(image, 1, -1);
		}

		private void Scaling(Image image)
		{
			Reflection(image, _scale, _scale);
		}

		private BitmapEncoder GetEncoder(string type)
		{
			BitmapEncoder encoder;

			switch (type)
			{
				case ".png":
					encoder = new PngBitmapEncoder();
					break;

				case ".jpeg":
					encoder = new JpegBitmapEncoder();
					break;

				case ".bmp":
					encoder = new BmpBitmapEncoder();
					break;

				default:
					encoder = new JpegBitmapEncoder();
					break;
			}

			return encoder;
		}

        private async void Contrast(Image image)
        {
			_brightnessAndContrasWindowService.Open();

            if (_brightnessAndContrasWindowService.DialogResult)
            {
                var bmDocument = new BMDocument(image.Source);
                await bmDocument.BrightnessAndContrast
					(_brightnessAndContrasWindowService.BrightnessValue, 
					_brightnessAndContrasWindowService.ContrastValue);
                image.Source = bmDocument.ConvertImageSourceToBitmap(bmDocument.CurrentBM);
            }
        }

        private async void InvertColors(Image image)
        {
	        var bmDocument = new BMDocument(image.Source);
	        await bmDocument.InvertColors();
	        image.Source = bmDocument.ConvertImageSourceToBitmap(bmDocument.CurrentBM);
        }

        private async void Sharp(Image image)
        {
	        var bmDocument = new BMDocument(image.Source);
	        await bmDocument.Sharp();
	        image.Source = bmDocument.ConvertImageSourceToBitmap(bmDocument.CurrentBM);
        }

		private async void Blur(Image image)
		{
			var bmDocument = new BMDocument(image.Source);
			await bmDocument.Blur();
			image.Source = bmDocument.ConvertImageSourceToBitmap(bmDocument.CurrentBM);
		}

		private async void Emboss(Image image)
		{
			var bmDocument = new BMDocument(image.Source);
			await bmDocument.Emboss();
			image.Source = bmDocument.ConvertImageSourceToBitmap(bmDocument.CurrentBM);
		}

		private async void Contour(Image image)
		{
			var bmDocument = new BMDocument(image.Source);
			await bmDocument.Countor();
			image.Source = bmDocument.ConvertImageSourceToBitmap(bmDocument.CurrentBM);
		}

		public MainWindowVM(IBrightnessAndContrastWindowService brightnessAndContrasWindowService)
		{
			AddImageCommand = new RelayCommand<Image>(AddImage);
			RightRotationCommand = new RelayCommand<Image>(RightRotation);
			LeftRotationCommand = new RelayCommand<Image>(LeftRotation);
			HorizontalReflectionCommand = new RelayCommand<Image>(HorizontalReflection);
			VerticalReflectionCommand = new RelayCommand<Image>(VerticalReflection);
			ScalingCommand = new RelayCommand<Image>(Scaling);
			SaveAsCommand = new RelayCommand<Image>(SaveAsFileDialog);
			SaveCommand = new RelayCommand<Image>(SaveFileDialog);

            BrightnessAndContrastCommand = new RelayCommand<Image>(Contrast);
			InvertColorsCommand = new RelayCommand<Image>(InvertColors);
			SharpCommand = new RelayCommand<Image>(Sharp);
			BlurCommand = new RelayCommand<Image>(Blur);
			EmbossCommand = new RelayCommand<Image>(Emboss);
			ContourCommand = new RelayCommand<Image>(Contour);

			_brightnessAndContrasWindowService = brightnessAndContrasWindowService;
        }
	}
}
