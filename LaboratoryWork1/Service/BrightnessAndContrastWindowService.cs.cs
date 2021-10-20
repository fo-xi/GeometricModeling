using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using LaboratoryWork1.Windows;

namespace LaboratoryWork1.Service
{
    public class BrightnessAndContrastWindowService : IBrightnessAndContrastWindowService
    {
        private BrightnessAndContrastWindow _brightnessAndContrastWindow;

        public bool DialogResult { get; set; }

        public void Open()
        {
            _brightnessAndContrastWindow = new BrightnessAndContrastWindow(new BrightnessAndContrastWindowVM(this));
            _brightnessAndContrastWindow.ShowDialog();
        }

        public RelayCommand OKCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }

        public int ContrastValue { get; set; }

        public int BrightnessValue { get; set; }

        private void OK()
        {
            var dataContext = _brightnessAndContrastWindow.DataContext as BrightnessAndContrastWindowVM;
            ContrastValue = dataContext.ContrastValue;
            BrightnessValue = dataContext.BrightnessValue;
            DialogResult = true;
            Close();
        }

        private void Cancel()
        {
            DialogResult = false;
            Close();
        }

        private void Close()
        {
            _brightnessAndContrastWindow.Close();
        }

        public BrightnessAndContrastWindowService()
        {
            OKCommand = new RelayCommand(OK);
            CancelCommand = new RelayCommand(Cancel);
        }
	}
}
