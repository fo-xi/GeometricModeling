using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.MobileControls;
using System.Windows;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using System.Drawing;
using System.Drawing.Imaging;
using LaboratoryWork1.Service;

namespace LaboratoryWork1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
	    MainWindowVM _mainWindow = new MainWindowVM(new BrightnessAndContrastWindowService());

        public MainWindow()
        {
            InitializeComponent();

            DataContext = _mainWindow;
        }
    }
}
