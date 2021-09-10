using System;
using System.Collections.Generic;
using System.IO;
using GalaSoft.MvvmLight;
using System.Windows;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Media;

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

				using (var fileStream = new FileStream(fileDialog.FileName, FileMode.Open))
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

			using (var fileStream = new FileStream(ImagePath, FileMode.Open))
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

		public MainWindowVM()
		{
			AddImageCommand = new RelayCommand<Image>(AddImage);
			RightRotationCommand = new RelayCommand<Image>(RightRotation);
			LeftRotationCommand = new RelayCommand<Image>(LeftRotation);
			HorizontalReflectionCommand = new RelayCommand<Image>(HorizontalReflection);
			VerticalReflectionCommand = new RelayCommand<Image>(VerticalReflection);
			ScalingCommand = new RelayCommand<Image>(Scaling);
			SaveAsCommand = new RelayCommand<Image>(SaveAsFileDialog);
			SaveCommand = new RelayCommand<Image>(SaveFileDialog);
		}
	}
}
