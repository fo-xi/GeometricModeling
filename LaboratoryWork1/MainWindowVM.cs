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
		private int Angle = 90;

		private double AxisX = 1;

		private double AxisY = 1;

		private string _imagePath;

		private double _scale;

		public RelayCommand<Image> AddImageCommand { get; set; }

		public RelayCommand<Image> RightRotationCommand { get; set; }

		public RelayCommand<Image> LeftRotationCommand { get; set; }

		public RelayCommand<Image> HorizontalReflectionCommand { get; set; }

		public RelayCommand<Image> VerticalReflectionCommand { get; set; }

		public RelayCommand<Image> ScalingCommand { get; set; }

		public RelayCommand SaveCommand { get; set; }

		public RelayCommand SaveAsCommand { get; set; }

		public string ImagePath
		{
			get
			{ return _imagePath; }
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
			var fileDialog = new OpenFileDialog()
			{
				Title = "Open Image",
				Filter = "*.png|*.png|*.bmp|*.bmp|*.jpeg|*.jpeg"
			};
			if ((bool)fileDialog.ShowDialog())
			{
				ImagePath = fileDialog.FileName;
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.UriSource = new Uri(ImagePath);
				bitmapImage.EndInit();
				image.Source = bitmapImage;
			}
		}

		private void Rotation(Image image)
		{
			TransformedBitmap transformedBitmap = new TransformedBitmap();

			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.UriSource = new Uri(ImagePath);
			bitmapImage.EndInit();

			transformedBitmap.BeginInit();
			transformedBitmap.Source = bitmapImage;
			RotateTransform transform = new RotateTransform(Angle);
			transformedBitmap.Transform = transform;
			transformedBitmap.EndInit();

			image.Source = transformedBitmap;
		}

		private void RightRotation(Image image)
		{
			Angle += 90;
			Rotation(image);
		}

		private void LeftRotation(Image image)
		{
			Angle -= 90;
			Rotation(image);
		}

		private void Reflection(Image image)
		{
			TransformedBitmap transformedBitmap = new TransformedBitmap();

			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.UriSource = new Uri(ImagePath);
			bitmapImage.EndInit();

			transformedBitmap.BeginInit();
			transformedBitmap.Source = bitmapImage;
			ScaleTransform transform = new ScaleTransform(AxisX, AxisY);
			transformedBitmap.Transform = transform;
			transformedBitmap.EndInit();

			image.Source = transformedBitmap;
		}

		private void HorizontalReflection(Image image)
		{
			AxisX *= -1;
			Reflection(image);
		}

		private void VerticalReflection(Image image)
		{
			AxisY *= -1;
			Reflection(image);
		}

		private void Scaling(Image image)
		{
			AxisX *= _scale;
			AxisY *= _scale;
			Reflection(image);
		}

		public void SaveAsFileDialog()
		{
			var fileDialog = new SaveFileDialog()
			{
				Title = "Save Image",
				Filter = "*.png|*.png|*.bmp|*.bmp|*.jpeg|*.jpeg"
			};

			if ((bool)fileDialog.ShowDialog())
			{
				ImagePath = fileDialog.FileName;
			}
		}

		public MainWindowVM()
		{
			AddImageCommand = new RelayCommand<Image>(AddImage);
			RightRotationCommand = new RelayCommand<Image>(RightRotation);
			LeftRotationCommand = new RelayCommand<Image>(LeftRotation);
			HorizontalReflectionCommand = new RelayCommand<Image>(HorizontalReflection);
			VerticalReflectionCommand = new RelayCommand<Image>(VerticalReflection);
			ScalingCommand = new RelayCommand<Image>(Scaling);
			SaveAsCommand = new RelayCommand(SaveAsFileDialog);
		}
	}
}
